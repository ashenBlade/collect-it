using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

[Collection("Open id dict tests")]
public class AuthorizationControllerTests : IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthorizationControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetToken_WithValidUsernameAndPassword_ShouldReturnIdToken()
    {
        var admin = PostgresqlCollectItDbContext.AdminUser;
        using var client = _factory.CreateClient();
        using var message = new HttpRequestMessage(HttpMethod.Post, "/connect/token");
        message.Content = new FormUrlEncodedContent(new[]
                                                    {
                                                        new KeyValuePair<string, string>("grant_type", "password"),
                                                        new KeyValuePair<string, string>("username", admin.UserName),
                                                        new KeyValuePair<string, string>("password", "12345678")
                                                    });
        
        var response = await client.SendAsync(message);
        var result = await response.Content.ReadFromJsonAsync<AccountDTO.OpenIddictResponseSuccess>();
        Assert.NotNull(result);
        Assert.Equal("Bearer", result.TokenType);
        Assert.NotEmpty(result.AccessToken);
    }
    
    private async Task<(HttpClient, string)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
                                                              username: username, 
                                                              password: password);
        return ( client, bearer );
    }

    [Fact]
    public async Task Register_WithValidValidValues_ShouldRegisterUser()
    {
        using var client = _factory.CreateClient();
        var username = "SomeValidUsername";
        var password = "SomeP@ssw0rd";
        var email = "test@mail.ru";
        var message = new HttpRequestMessage(HttpMethod.Post, "connect/register")
                      {
                          Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                                                              {
                                                                  new ("email", email),
                                                                  new ("password", password),
                                                                  new ("username", username)
                                                              })
                      };
        var response = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var actual = await  response.Content.ReadFromJsonAsync<AccountDTO.ReadUserDTO>();
        Assert.NotNull(actual);
        Assert.Equal(username, actual!.UserName);
        Assert.Equal(email, actual.Email);
    }
    
    [Fact]
    public async Task CreateUser_AlreadyRegisteredUsername_ShouldReturnBadRequest()
    {
        using var client = _factory.CreateClient();
        var user = PostgresqlCollectItDbContext.AdminUser;
        var dto = new AccountDTO.CreateUserDTO(user.UserName, "oleg@mail.ru", "P@ssw0rd2H@rd");
        using var message = new HttpRequestMessage(HttpMethod.Post, "connect/register")
                            {
                                Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                                                                    {
                                                                        new("username", dto.UserName),
                                                                        new("email", dto.Email),
                                                                        new("password", dto.Password)
                                                                    }),
                            };
        var response = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}