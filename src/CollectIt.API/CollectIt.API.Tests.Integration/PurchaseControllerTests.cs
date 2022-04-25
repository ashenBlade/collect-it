using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

[Collection("Purchase")]
public class PurchaseControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public PurchaseControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
    }
    
    private async Task<(HttpClient, string)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
                                                              helper: _outputHelper, 
                                                              username: username, 
                                                              password: password);
        return ( client, bearer );
    }

    [Fact]
    public async Task PurchaseSubscription_WithValidSubscriptionId_ShouldSubscribeUser()
    {
        var user = PostgresqlCollectItDbContext.AdminUser;
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        var subscription = PostgresqlCollectItDbContext.SilverSubscription;
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO>(client,
                                                                                                    $"api/v1/purchase/subscription/{subscription.Id}",
                                                                                                    bearer,
                                                                                                    HttpMethod.Post,
                                                                                                    _outputHelper);
        Assert.Equal(subscription.Id, actual.SubscriptionId);
        Assert.True(actual.DateFrom < actual.DateTo);
        Assert.True(0 < actual.LeftResourcesCount);
        client.Dispose();
    }
    
    [Fact]
    public async Task PurchaseSubscription_WithUnexistingSubscriptionId_ShouldReturnNotFound()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        await TestsHelpers.AssertStatusCodeAsync(client,
                                     $"api/v1/purchase/subscription/10", HttpStatusCode.NotFound, HttpMethod.Post,
                                     bearer);
        client.Dispose();
    }
    
    [Fact]
    public async Task PurchaseSubscription_WithSameExistingActiveSubscription_ShouldReturnBadRequest()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        var subscription = PostgresqlCollectItDbContext.SilverSubscription;
        await TestsHelpers.SendAsync(client, $"api/v1/purchase/subscription/{subscription.Id}", bearer, null,
                                     _outputHelper, true, HttpMethod.Post);
        await TestsHelpers.AssertStatusCodeAsync(client, $"api/v1/purchase/subscription/{subscription.Id}",
                                                 HttpStatusCode.BadRequest, HttpMethod.Post, bearer);
        client.Dispose();
    }
    
    
    [Fact]
    public async Task PurchaseImage_WithValidImageId_ShouldPurchaseImage()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var image = PostgresqlCollectItDbContext.DefaultImages.First();
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        await TestsHelpers.SendAsync(client,
                                     $"api/v1/purchase/image/{image.Id}", 
                                     bearer, 
                                     null, 
                                     _outputHelper, 
                                     true, 
                                     HttpMethod.Post);
        var acquiredUserResources =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadAcquiredUserResourceDTO[]>(client,
                                                                                             $"api/v1/users/{user.Id}/images",
                                                                                             bearer, 
                                                                                             HttpMethod.Get,
                                                                                             _outputHelper);
        Assert.NotEmpty(acquiredUserResources);
        Assert.Contains(acquiredUserResources, dto => dto.Id != image.Id || dto.Name == image.Name);
        client.Dispose();
    }

    [Fact]
    public async Task PurchaseImage_WithAlreadyAcquiredImage_ShouldReturnBadRequest()
    {
        var user = PostgresqlCollectItDbContext.AdminUser;
        var image = PostgresqlCollectItDbContext.DefaultImages.First();
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        await TestsHelpers.SendAsync(client,
                                     $"api/v1/purchase/image/{image.Id}", 
                                     bearer, 
                                     null, 
                                     _outputHelper, 
                                     true, 
                                     HttpMethod.Post);
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/purchase/image/{image.Id}",
                                                 HttpStatusCode.BadRequest,
                                                 HttpMethod.Post,
                                                 bearer);
        client.Dispose();
    }
    
    [Fact]
    public async Task PurchaseImage_WithNoSuitableSubscription_ShouldReturnBadRequest()
    {
        var user = PostgresqlCollectItDbContext.TechSupportUser;
        var image = PostgresqlCollectItDbContext.DefaultImages.First();
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/purchase/image/{image.Id}",
                                                 HttpStatusCode.BadRequest,
                                                 HttpMethod.Post,
                                                 bearer);
        client.Dispose();
    }
    
    [Fact]
    public async Task PurchaseImage_WithUnexistingResourceId_ShouldReturnBadRequest()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserTwo;
        var image = PostgresqlCollectItDbContext.DefaultImages.First();
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/purchase/image/{image.Id}",
                                                 HttpStatusCode.BadRequest,
                                                 HttpMethod.Post,
                                                 bearer);
        client.Dispose();
    }
}