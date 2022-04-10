using System.Net;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
using Xunit;

namespace CollectIt.API.Tests.Integration;

public class SubscriptionsControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;

    public SubscriptionsControllerTests(CollectItWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSubscriptionsList_ShouldReturnArrayOfSubscriptions()
    {
        var subscriptions =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO[]>(_factory,
                                                                                         $"api/v1/subscriptions?type={ResourceType.Image}&page_size=10&page_number=1");
        Assert.NotEmpty(subscriptions);
    }

    [Fact]
    public async Task GetSubscriptionById_WithValidId_ShouldReturnRequiredSubscription()
    {
        var subscription =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO>(_factory,
                                                                                       "api/v1/subscriptions/1");
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
        var subscriptions = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadSubscriptionDTO[]>(_factory,
                                                                                                         "api/v1/subscriptions/active?type=1&page_size=10&page_number=1");
        Assert.NotEmpty(subscriptions);
    }
}

