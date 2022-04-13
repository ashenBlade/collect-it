using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class UsersControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public UsersControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task GetUsersList_Return4InitialUsers()
    {
        var users = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO[]>(_factory, "api/v1/users");
        Assert.Equal(4, users.Length);
    }

    [Fact]
    public async Task GetUserById_WithCorrectIdForExistingUser_ReturnRequiredUser()
    {
        var expected = PostgresqlCollectItDbContext.AdminUser;
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(_factory, $"api/v1/users/{expected.Id}");
        Assert.NotNull(actual);
        Assert.Equal(expected.UserName, actual.UserName);
        Assert.Equal(expected.Email, actual.Email);
    }

    [Fact]
    public async Task GetUserById_WithIdForNotExistingUser_ReturnNotFound()
    {
        await TestsHelpers.AssertNotFoundAsync(_factory, "api/v1/users/7", HttpMethod.Get);
    }

    [Fact]
    public async Task GetUsersSubscriptionsList_WithValidUserId_ReturnListOfSubscriptions()
    {
        var result = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO[]>(_factory, "api/v1/users/1/subscriptions");
        Assert.NotNull(result);
    }


    [Fact]
    public async Task GetActiveUserSubscription_WithIdForUserWithActiveSubscriptions_ShouldReturnNotEmptyArrayOfUserSubscription()
    {
        var result =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO[]>(_factory, "api/v1/users/1/active-subscriptions");
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }


    [Fact]
    public async Task GetUserRoles_WithAdminId_ShouldReturnArrayContainingAdminRoleString()
    {
        var result = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO[]>(_factory, "api/v1/users/1/roles");
        Assert.True(result.Length > 0);
        Assert.Contains(result, r => r.Name == "Admin");
    }

    [Fact]
    public async Task PostUsername_WithNewValidName_ShouldChangeUsername()
    {
        var userId = PostgresqlCollectItDbContext.AdminUserId;
        const string expectedNewUsername = "SomeNewUsername";
        await TestsHelpers.PostAsync(_factory, $"api/v1/users/{userId}/username?username={expectedNewUsername}");
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(_factory, $"api/v1/users/{userId}", outputHelper: _outputHelper);
        Assert.Equal(expectedNewUsername, actual.UserName);
    }
}