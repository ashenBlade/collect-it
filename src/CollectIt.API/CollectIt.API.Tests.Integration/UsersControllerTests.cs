using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
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
        var (client, bearer) = await Initialize();
        const string expectedNewUsername = "SomeUserName";
        var user = PostgresqlCollectItDbContext.AdminUser;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/username", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(expectedNewUsername), "username"}
                                     }, _outputHelper, method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}", bearer);
        Assert.Equal(expectedNewUsername, actual.UserName);
        client.Dispose();
    }

    [Fact]
    public async Task PostEmail_WithNewValidEmail_ShouldChangeEmail()
    {
        var (client, bearer) = await Initialize();
        const string expectedNewEmail = "thisisbrandnewemail@mail.ru";
        var user = PostgresqlCollectItDbContext.AdminUser;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/email", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(expectedNewEmail), "email"}
                                     },
                                     _outputHelper,
                                     method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}",
                                                                               bearer);
        Assert.Equal(expectedNewEmail, actual.Email);
        client.Dispose();
    }

    [Fact]
    public async Task AssignRole_WithValidRole_ShouldAssignUserToNewRole()
    {
        var (client, bearer) = await Initialize();
        var roleToAssign = Role.TechSupportRoleName;
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/roles", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(roleToAssign), "role_name"}
                                     },
                                     _outputHelper,
                                     method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}",
                                                                               bearer);
        Assert.Contains(roleToAssign, actual.Roles);
        client.Dispose();
    }
    
    [Fact]
    public async Task DeleteRole_WithAssignedRole_ShouldRemoveRoleFromUser()
    {
        var (client, bearer) = await Initialize();
        var roleToRemove = Role.TechSupportRoleName;
        var user = PostgresqlCollectItDbContext.TechSupportUser;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/roles", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(roleToRemove), "role_name"}
                                     },
                                     _outputHelper);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}",
                                                                               bearer);
        Assert.DoesNotContain(roleToRemove, actual.Roles);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteRole_WithAssignedRoleAndUserNotInAdminRole_ShouldReturnForbidden()
    {
        var (client, bearer) = await Initialize(PostgresqlCollectItDbContext.DefaultUserOne.UserName, "12345678");
        var roleToRemove = Role.TechSupportRoleName;
        var user = PostgresqlCollectItDbContext.TechSupportUser;
        var userId = user.Id;
        using var message = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/users/{userId}/roles")
                            {
                                Content = new FormUrlEncodedContent(new[]
                                                                    {
                                                                        new KeyValuePair<string, string>("role_name",
                                                                                                         roleToRemove)
                                                                    }),
                                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", bearer)}
                            };
        var response = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        client.Dispose();
    }

    [Fact]
    public async Task ActivateUserAccount_WithDeactivatedAccount_ShouldReturnNoContent()
    {
        var (client, bearer) = await Initialize();
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var response = await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/activate", bearer,
                                                    outputHelper: _outputHelper, ensure: true, method: HttpMethod.Post);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeactivateUserAccount_WithActivatedAccount_ShouldReturnNoContent()
    {
        var (client, bearer) = await Initialize();
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var response = await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/activate", bearer,
                                                    outputHelper: _outputHelper, ensure: true, method: HttpMethod.Post);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}