using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class SubscriptionsControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private async Task<(HttpClient, string)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
                                                              helper: _outputHelper, 
                                                              username: username, 
                                                              password: password);
        return ( client, bearer );
    }
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public SubscriptionsControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task GetSubscriptionsList_ShouldReturnArrayOfSubscriptions()
    {
        var (client, bearer) = await Initialize();
        var subscriptions =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO[]>(client,
                                                                                         $"api/v1/subscriptions?type={ResourceType.Image}&page_size=10&page_number=1",
                                                                                         bearer);
        Assert.NotEmpty(subscriptions);
    }

    [Fact]
    public async Task GetSubscriptionById_WithValidId_ShouldReturnRequiredSubscription()
    {
        var (client, bearer) = await Initialize();
        var subscription =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                       "api/v1/subscriptions/1",
                                                                                       bearer);
        Assert.Equal(1, subscription.Id);
    }

    [Fact]
    public async Task GetSubscriptionById_WithNonExistingId_ShouldReturnNotFoundStatusCode()
    {
        await TestsHelpers.AssertStatusCodeAsync(_factory, "api/v1/subscriptions/9", HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveSubscriptions_WithValidQueryParameters_ShouldReturnListOfSubscriptions()
    {
        var (client, bearer) = await Initialize();
        var subscriptions = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO[]>(client,
                                                                                                         "api/v1/subscriptions/active?type=1&page_size=10&page_number=1",
                                                                                                         bearer);
        client.Dispose();
        Assert.NotEmpty(subscriptions);
    }

    [Fact]
    public async Task ChangeSubscriptionName_WithValidName_ShouldChangeName()
    {
        var (client, bearer) = await Initialize();
        var subscription = PostgresqlCollectItDbContext.BronzeSubscription;
        const string newSubscriptionName = "Another subscription name";
        await TestsHelpers.SendAsync(client, $"api/v1/subscriptions/{subscription.Id}/name",
                                     bearer,
                                     new FormUrlEncodedContent(new[]
                                                               {
                                                                   new KeyValuePair<string, string>("name",
                                                                                                    newSubscriptionName)
                                                               }), method: HttpMethod.Post,
                                     outputHelper: _outputHelper);
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                                $"api/v1/subscriptions/{subscription.Id}",
                                                                                                bearer, HttpMethod.Get,
                                                                                                _outputHelper);
        Assert.Equal(newSubscriptionName, actual.Name);
    }
    
    [Fact]
    public async Task ChangeSubscriptionDescription_WithValidDescription_ShouldChangeDescription()
    {
        var (client, bearer) = await Initialize();
        var subscription = PostgresqlCollectItDbContext.BronzeSubscription;
        const string newDescription = "Another subscription description";
        await TestsHelpers.SendAsync(client, $"api/v1/subscriptions/{subscription.Id}/description",
                                     bearer,
                                     new FormUrlEncodedContent(new[]
                                                               {
                                                                   new KeyValuePair<string, string>("description",
                                                                                                    newDescription)
                                                               }), method: HttpMethod.Post,
                                     outputHelper: _outputHelper);
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                                $"api/v1/subscriptions/{subscription.Id}",
                                                                                                bearer, 
                                                                                                HttpMethod.Get,
                                                                                                _outputHelper);
        Assert.Equal(newDescription, actual.Description);
    }

    [Fact]
    public async Task DeactivateSubscription_WithValidId_ShouldDeactivateSubscription()
    {
        var (client, bearer) = await Initialize();
        var subscription = PostgresqlCollectItDbContext.BronzeSubscription;
        await TestsHelpers.SendAsync(client, 
                                     $"api/v1/subscriptions/{subscription.Id}/deactivate",
                                     bearer, 
                                     method: HttpMethod.Post,
                                     outputHelper: _outputHelper);
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                                $"api/v1/subscriptions/{subscription.Id}",
                                                                                                bearer, 
                                                                                                HttpMethod.Get,
                                                                                                _outputHelper);
        Assert.False(actual.Active);
    }
    
    [Fact]
    public async Task ActivateSubscription_WithValidIdAndDeactivatedSubscription_ShouldActivateSubscription()
    {
        var (client, bearer) = await Initialize();
        // Single deactivated subscription that must be deactivated by default
        var subscription = PostgresqlCollectItDbContext.AllowAllSubscription;
        var before = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                                $"api/v1/subscriptions/{subscription.Id}",
                                                                                                bearer,
                                                                                                HttpMethod.Get,
                                                                                                _outputHelper);
        Assert.False(before.Active);
        await TestsHelpers.SendAsync(client, 
                                     $"api/v1/subscriptions/{subscription.Id}/activate",
                                     bearer, 
                                     method: HttpMethod.Post,
                                     outputHelper: _outputHelper);
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                                $"api/v1/subscriptions/{subscription.Id}",
                                                                                                bearer, 
                                                                                                HttpMethod.Get,
                                                                                                _outputHelper);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task CreateSubscription_WithValidSubscriptionData_ShouldCreateSubscription()
    {
        var subscription = new AccountDTO.CreateSubscriptionDTO()
                           {
                               Description = "Brand new subscription for elite members",
                               Name = "Sample text",
                               AppliedResourceType = ResourceType.Video,
                               MaxResourcesCount = 10,
                               MonthDuration = 2,
                               Price = 100,
                               Restriction = null
                           };
        var (client, bearer) = await Initialize();
        var result = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                  "api/v1/subscriptions",
                                                  bearer,
                                                  HttpMethod.Post,
                                                  content: new FormUrlEncodedContent(new[]
                                                                            {
                                                                                new KeyValuePair<string, string>("Name",
                                                                                                                 subscription
                                                                                                                    .Name),
                                                                                new KeyValuePair<string,
                                                                                    string>("Description",
                                                                                            subscription.Description),
                                                                                new KeyValuePair<string,
                                                                                    string>("Price",
                                                                                            subscription.Price
                                                                                                        .ToString()),
                                                                                new KeyValuePair<string,
                                                                                    string>("MonthDuration",
                                                                                            subscription.MonthDuration
                                                                                                        .ToString()),
                                                                                new KeyValuePair<string,
                                                                                    string>("MaxResourcesCount",
                                                                                            subscription
                                                                                               .MaxResourcesCount
                                                                                               .ToString()),
                                                                                new KeyValuePair<string,
                                                                                    string>("AppliedResourceType",
                                                                                            ((int)subscription
                                                                                                   .AppliedResourceType
                                                                                            ).ToString()),
                                                                                new KeyValuePair<string,
                                                                                    string>("RestrictionId",
                                                                                            subscription.Restriction
                                                                                                       ?.ToString()
                                                                                         ?? string.Empty)
                                                                            }),
                                                  outputHelper: _outputHelper);
        var createdId = result.Id;
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(client,
                                                                                       $"api/v1/subscriptions/{createdId}",
                                                                                       bearer, HttpMethod.Get,
                                                                                       _outputHelper);
        Assert.Equal(createdId, actual.Id);
        Assert.Equal(subscription.Name, actual.Name);
        Assert.Equal(subscription.Description, actual.Description);
        Assert.Equal(subscription.Price, actual.Price);
        Assert.Equal(subscription.MonthDuration, actual.MonthDuration);
        Assert.Equal(subscription.AppliedResourceType, actual.AppliedResourceType);
    }
}

