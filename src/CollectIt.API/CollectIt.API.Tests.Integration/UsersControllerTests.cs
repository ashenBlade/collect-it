using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class UsersControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public UsersControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetUsersList_Return4InitialUsers()
    {
        var users = await GetResultParsedFromJson<AccountDTO.ReadUserDTO[]>("api/v1/users");
        Assert.Equal(4, users.Length);
    }

    [Fact]
    public async Task GetUserById_WithCorrectIdForExistingUser_ReturnRequiredUser()
    {
        var user = await GetResultParsedFromJson<AccountDTO.ReadUserDTO>("api/v1/users/1");
        _testOutputHelper.WriteLine(user.UserName);
    }

    [Fact]
    public async Task GetUserById_WithIdForNotExistingUser_ReturnNotFound()
    {
        using var client = _factory.CreateClient();
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/v1/users/7");
        var request = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
    }

    [Fact]
    public async Task GetUsersSubscriptionsList_WithValidUserId_ReturnListOfSubscriptions()
    {
        var result = await GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO[]>("api/v1/users/1/subscriptions");
        Assert.NotNull(result);
    }


    [Fact]
    public async Task GetActiveUserSubscription_WithIdForUserWithActiveSubscriptions_ShouldReturnNotEmptyArrayOfUserSubscription()
    {
        var result =
            await
                GetResultParsedFromJson<AccountDTO.ReadActiveUserSubscription[]>("api/v1/users/1/active-subscriptions");
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }


    [Fact]
    public async Task GetUserRoles_WithAdminId_ShouldReturnArrayContainingAdminRoleString()
    {
        var result = await GetResultParsedFromJson<AccountDTO.ReadRoleDTO[]>("api/v1/users/1/roles");
        Assert.True(result.Length > 0);
        Assert.Contains(result, r => r.Name == "Admin");
    }

    private async Task<T> GetResultParsedFromJson<T>(string address, HttpMethod? method = null)
    {
        using var client = _factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        var result = await client.SendAsync(message);
        var json = await result.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(json);
        var serializer = JsonSerializer.Create();
        var parsed = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
        Assert.NotNull(parsed);
        return parsed;
    }
}