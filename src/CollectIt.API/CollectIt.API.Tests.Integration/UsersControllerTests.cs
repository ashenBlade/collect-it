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
        var (client, bearer) = await Initialize();
        var users = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO[]>(client, "api/v1/users", bearer, HttpMethod.Get, _outputHelper);
        client.Dispose();
        Assert.Equal(4, users.Length);
    }

    [Fact]
    public async Task GetUserById_WithCorrectIdForExistingUser_ReturnRequiredUser()
    {
        var (client, bearer) = await Initialize();
        var expected = PostgresqlCollectItDbContext.AdminUser;
        
        var actual = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{expected.Id}", bearer, HttpMethod.Get, _outputHelper);
        client.Dispose();
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
        var (client, bearer) = await Initialize();
        var result = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO[]>(client, "api/v1/users/1/subscriptions", bearer, HttpMethod.Get, _outputHelper);
        client.Dispose();
        Assert.NotNull(result);
    }


    [Fact]
    public async Task GetActiveUserSubscription_WithIdForUserWithActiveSubscriptions_ShouldReturnNotEmptyArrayOfUserSubscription()
    {
        var (client, bearer) = await Initialize();
        var result =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserSubscriptionDTO[]>(client, "api/v1/users/1/active-subscriptions", bearer, HttpMethod.Get, _outputHelper);
        client.Dispose();
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }


    [Fact]
    public async Task GetUserRoles_WithAdminId_ShouldReturnArrayContainingAdminRoleString()
    {
        var (client, bearer) = await Initialize();
        var result = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO[]>(client, 
                                                                                          "api/v1/users/1/roles",
                                                                                          bearer,
                                                                                          HttpMethod.Get,
                                                                                          _outputHelper);
        client.Dispose();
        Assert.True(result.Length > 0);
        Assert.Contains(result, r => r.Name == "Admin");
    }

    [Fact]
    public async Task PostUsername_WithNewValidName_ShouldChangeUsername()
    {
        var (client, bearer) = await Initialize();
        const string expectedNewUsername = "SomeUserName";
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
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
    public async Task PostUsername_WithNewInvalidNameContainingWhitespaces_ShouldReturnBadRequest()
    {
        var (client, bearer) = await Initialize();
        const string invalidUsername = "Invalid username";
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/users/{userId}/username",
                                                 HttpStatusCode.BadRequest,
                                                 HttpMethod.Post,
                                                 bearer,
                                                 new MultipartFormDataContent()
                                                 {
                                                     {new StringContent(invalidUsername), "username"}
                                                 });
        client.Dispose();
    }
    
    [Fact]
    public async Task PostUsername_WithNewValidNameAndUserAsRequester_ShouldChangeUsername()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var (client, bearer) = await Initialize(user.UserName, "12345678");
        const string expectedNewUsername = "SomeUserName";
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/username", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(expectedNewUsername), "username"}
                                     }, 
                                     _outputHelper, 
                                     method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}", bearer);
        Assert.Equal(expectedNewUsername, actual.UserName);
        client.Dispose();
    }
    
    [Fact]
    public async Task PostUsername_WithNonexistentUserId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        const string expectedNewUsername = "SomeUserName";
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 "api/v1/users/80/username", 
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Post,
                                                 bearer,
                                                 new MultipartFormDataContent()
                                                 {
                                                     {new StringContent(expectedNewUsername), "username"}
                                                 });
        client.Dispose();
    }
    
    [Fact]
    public async Task PostUsername_WithValidUsernameAndAdminAsRequester_ShouldChangeUsername()
    {
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var (client, bearer) = await Initialize();
        const string expectedNewUsername = "SomeUserName";
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/username", bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(expectedNewUsername), "username"}
                                     }, 
                                     _outputHelper, 
                                     method: HttpMethod.Post);
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
        var user = PostgresqlCollectItDbContext.DefaultUserTwo;
        var userId = user.Id;
        var result = await TestsHelpers.SendAsync(client, 
                                                  $"api/v1/users/{userId}/email", 
                                                  bearer,
                                                  new FormUrlEncodedContent(new[]{new KeyValuePair<string, string>("email", expectedNewEmail)}),
                                                  _outputHelper,
                                                  method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, 
                                                                               $"api/v1/users/{userId}",
                                                                               bearer, 
                                                                               HttpMethod.Get);
        client.Dispose();
        Assert.Equal(expectedNewEmail, actual.Email);
    }
    
    [Fact]
    public async Task PostEmail_WithInvalidEmail_ShouldReturnBadRequest()
    {
        var (client, bearer) = await Initialize();
        const string invalidEmail = "invalid-email-without 'at' sign";
        var user = PostgresqlCollectItDbContext.DefaultUserTwo;
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 $"api/v1/users/{user.Id}/email", HttpStatusCode.BadRequest,
                                                 HttpMethod.Post, bearer, 
                                                 new FormUrlEncodedContent(new[]
                                                                           {
                                                                               new KeyValuePair
                                                                                   <string,
                                                                                   string>("email",
                                                                                           invalidEmail        )
                                                                           }));
        client.Dispose();
    }
    
    [Fact]
    public async Task PostEmail_WithRequesterNotUserItselfOrAdmin_ShouldReturnForbidden()
    {
        var requester = PostgresqlCollectItDbContext.DefaultUserTwo;
        var userToChange = PostgresqlCollectItDbContext.DefaultUserOne;
        var (client, bearer) = await Initialize(requester.UserName, "12345678");
        const string newEmail = "sometotalynewemail@mail.ru";
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 $"api/v1/users/{userToChange.Id}/email", 
                                                 HttpStatusCode.Forbidden,
                                                 HttpMethod.Post, 
                                                 bearer, 
                                                 new FormUrlEncodedContent(new[]
                                                                           {
                                                                               new KeyValuePair
                                                                                   <string,
                                                                                   string>("email",
                                                                                           newEmail)
                                                                           }));
        client.Dispose();
    }
    
    
    [Fact]
    public async Task AssignRole_WithValidRole_ShouldAssignUserToNewRole()
    {
        var (client, bearer) = await Initialize();
        var roleToAssign = Role.TechSupportRoleName;
        var user = PostgresqlCollectItDbContext.DefaultUserTwo;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, 
                                     $"api/v1/users/{userId}/roles", 
                                     bearer,
                                     new FormUrlEncodedContent(new[]{new KeyValuePair<string, string>("role_name", roleToAssign)}),
                                     _outputHelper,
                                     method: HttpMethod.Post);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, 
                                                                               $"api/v1/users/{userId}",
                                                                               bearer);
        client.Dispose();
        Assert.Contains(roleToAssign, actual.Roles);
    }
    
    [Fact]
    
    public async Task DeleteRole_WithAssignedRole_ShouldRemoveRoleFromUser()
    {
        var (client, bearer) = await Initialize();
        var roleToRemove = Role.TechSupportRoleName;
        var user = PostgresqlCollectItDbContext.TechSupportUser;
        var userId = user.Id;
        await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/roles", 
                                     bearer,
                                     new MultipartFormDataContent()
                                     {
                                         {new StringContent(roleToRemove), "role_name"}
                                     },
                                     _outputHelper,
                                     method: HttpMethod.Delete);
        var actual =
            await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadUserDTO>(client, $"api/v1/users/{userId}",
                                                                               bearer);
        client.Dispose();
        Assert.DoesNotContain(roleToRemove, actual.Roles);
    }

    [Fact]
    public async Task DeleteRole_WithAssignedRoleAndUserNotInAdminRole_ShouldReturnForbidden()
    {
        var (client, bearer) = await Initialize(PostgresqlCollectItDbContext.TechSupportUser.UserName, "12345678");
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
        client.Dispose();
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ActivateUserAccount_WithDeactivatedAccount_ShouldReturnNoContent()
    {
        var (client, bearer) = await Initialize();
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var response = await TestsHelpers.SendAsync(client, 
                                                    $"api/v1/users/{userId}/activate", 
                                                    bearer,
                                                    outputHelper: _outputHelper, 
                                                    ensure: true, 
                                                    method: HttpMethod.Post);
        client.Dispose();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeactivateUserAccount_WithActivatedAccount_ShouldReturnNoContent()
    {
        var (client, bearer) = await Initialize();
        var user = PostgresqlCollectItDbContext.DefaultUserOne;
        var userId = user.Id;
        var response = await TestsHelpers.SendAsync(client, $"api/v1/users/{userId}/deactivate", bearer,
                                                    outputHelper: _outputHelper, ensure: true, method: HttpMethod.Post);
        client.Dispose();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveRole_WithUnexistingUser_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 "api/v1/users/1000/roles", 
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Delete, 
                                                 bearer,
                                                 new FormUrlEncodedContent(new[]{new KeyValuePair<string, string>("role_name", Role.TechSupportRoleName)}));
        client.Dispose();
    }
    
    [Fact]
    public async Task AssignRole_WithUnexistingUser_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 "api/v1/users/1000/roles", 
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Post, 
                                                 bearer,
                                                 new FormUrlEncodedContent(new[]{new KeyValuePair<string, string>("role_name", Role.TechSupportRoleName)}));
        client.Dispose();
    }
    
    [Fact]
    public async Task ActivateAccount_WithUnexistingUser_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 "api/v1/users/1000/activate", 
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Post, 
                                                 bearer);
        client.Dispose();
    }
    [Fact]
    public async Task DeactivateAccount_WithUnexistingUser_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client, "api/v1/users/1000/deactivate", HttpStatusCode.NotFound,
                                                 HttpMethod.Post, bearer);
        client.Dispose();
    }
}