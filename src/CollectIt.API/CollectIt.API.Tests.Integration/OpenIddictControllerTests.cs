using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class OpenIddictControllerTests : IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public OpenIddictControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
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
}