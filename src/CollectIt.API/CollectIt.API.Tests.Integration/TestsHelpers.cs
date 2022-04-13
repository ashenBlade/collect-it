using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CollectIt.Database.Infrastructure;
using Microsoft.AspNetCore.Mvc.Formatters;
// using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;

namespace CollectIt.API.Tests.Integration;

public static class TestsHelpers
{
    public static async Task<string> GetBearerForUserAsync(HttpClient client, string? username = null, string? password = null, ITestOutputHelper? helper = null)
    {
        username ??= PostgresqlCollectItDbContext.AdminUser.UserName;
        password ??= "12345678";
        var message = new HttpRequestMessage(HttpMethod.Post, "connect/token")
                      {
                          Content = new FormUrlEncodedContent(new[]
                                                              {
                                                                  new KeyValuePair<string, string>("grant_type", "password"),
                                                                  new KeyValuePair<string, string>("username", username),
                                                                  new KeyValuePair<string, string>("password", password)
                                                              })
                      };
        var response = await client.SendAsync(message);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            helper?.WriteLine(await response.Content.ReadAsStringAsync());
            throw;
        }
        var token = await response.Content.ReadFromJsonAsync<ConnectResult>();
        return token.Bearer;
    }

    public static string AdminAccessTokenBearer => PostgresqlCollectItDbContext.AdminAccessTokenBearer;
    
    public static async Task<T> GetResultParsedFromJson<T>(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer);
        var result = await client.SendAsync(message);
        var parsed = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(parsed);
        return parsed;
    }
    
    public static async Task<T> GetResultParsedFromJson<T>(HttpClient client,
                                                           string address,
                                                           string bearer,
                                                           HttpMethod? method = null,
                                                           ITestOutputHelper? outputHelper = null)
    {
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", bearer);
        var result = await client.SendAsync(message);
        var json = await result.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(parsed);
        return parsed;
    }

    public static async Task PostAsync(CollectItWebApplicationFactory factory, string address, MultipartFormDataContent? form = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(HttpMethod.Post, address)
                            {
                                Headers =
                                {
                                    Authorization = new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer)
                                },
                                Content = form
                            };
        var result = await client.SendAsync(message);
        try
        {
            result.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
        }
    }
    
    public static async Task PostAsync(HttpClient client,
                                       string address,
                                       string bearer,
                                       MultipartFormDataContent? form = null,
                                       ITestOutputHelper? outputHelper = null)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, address)
                            {
                                Headers =
                                {
                                    Authorization = new AuthenticationHeaderValue("Bearer", bearer)
                                },
                                Content = form
                            };
        var result = await client.SendAsync(message);
        try
        {
            result.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
            throw;
        }
    }

    public static Task AssertNotFoundAsync(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null)
    {
        return AssertStatusCodeAsync(factory, address, HttpStatusCode.NotFound, method);
    }

    public static async Task AssertStatusCodeAsync(CollectItWebApplicationFactory factory, string address, HttpStatusCode expectedStatusCode, HttpMethod? method = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer);
        var result = await client.SendAsync(message);
        Assert.Equal(expectedStatusCode, result.StatusCode);
    }
}