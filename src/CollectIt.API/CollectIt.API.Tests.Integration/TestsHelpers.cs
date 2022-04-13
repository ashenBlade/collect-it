using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollectIt.Database.Infrastructure;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public static class TestsHelpers
{
    public static async Task<string> GetBearerForUserAsync(HttpClient client, string? username = null, string? password = null)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "connect/token")
                      {
                          Content = new MultipartFormDataContent()
                                    {
                                        {new StringContent("password"), "grant_type"},
                                        {new StringContent(username ?? PostgresqlCollectItDbContext.AdminUser.UserName), "username"},
                                        {new StringContent(password ?? "12345678"), "password"}
                                    }
                      };
        var response = await client.SendAsync(message);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<ConnectResult>();
        return token.Bearer;
    }
    public static string AdminAccessTokenBearer =>
        "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIiLCJraWQiOiJEN0E1NDZFNzFDRDA0MkI3MzkyMzVCOTU0MjBDOUQ1OEFGRDE4QUZCIiwidHlwIjoiYXQrand0In0.B0tSorozswXkU7aB6kIHiWTSOzba--wlaoa3zo7IUozyveVDMRV8gumGHzxRqwAbOEwDixOc9QJ3AsG2xxEN457oZqm7arvV2AZUA-tpyX5DABwZRBQwwhigZjkWzr5x461sgjxwdj64YVRp-GaBv95ESPTv7qV2eSJFVTAL2EpWZvAYqbkdOK4YEP2ZO_hYTBWQ1I4WqmZq2UFB2i4ByveIpZM2f0lAPhmP3HROkWEtdpyrDajHTJ1bbvrtD1rk-ZG_vIG7uy_zjN_-PtSqbdfFVnyG9xac_xuPEdU-ZBR-MIT17iHZ2lYq1O9uDUNfzdXQUkFMFJdrhjkHrEX_1Q.ZzbHGXhDuPmwRwehWpPegA.ojsJQTIHubmlMOCLGQJ2ZR80pzt-nf4ElgL3_VeU4joNd1WUNiS4beEdkuX16KxZ1wxGK3v_UBni_Cs_Y77niqZQMXFpCc9iTxUzyW4kVUfNZqx2RWhV8BBVBdvWg81D6GS2oi40St-mMR0Pg1NUrbEwBjT312Ci4k9MMPlcKr7l6uWfhrAHKPDTCcwjbhUcgX2owj9IVJRumWmDwkl9NQdXTxAr1tJuxkmg8euspEQ3WCpAKe03Jp6sfZjbiICSe5914ld4KowOrMM8fLlzlLJSLC5a4OHGk20mSAlp9mJTRq1jbl25mHUqoqW0DYAkEFsR3O2fngVE0mMYX7D5USsN9zpD21OXUbRkSzyxKVBsi4RwWb1lEy6yn-2CGilGi8lY53y1AwOThUEbFnxqxZBEPHqYMDce6YHinbOY9Yp1JrFLb_q_tUqKFYknsunvPvzCl0LgkqtO63Y3tXPrJrRh4Ui7KaXrzETc3Wscn0YvtnHgBiLo6tIGBP0WIIb8exeBq3RjDKtCdREglq9a2xJGsu_zRZ-KNs3sl8W4KwB3EkyMN3VhNv7JV54-IbbbYfZm6l8WltPw_WBa63KBLcDPKvLFsIuxjVXoQIwT1QxZT7Qm1fB2yLuLzGZYJgNKmEAGZgNnXzqG-opxvBLbD5I1Zy7jSbO7ssnL1zC1UMFPRRVnxOA8x8veGy7wAqBwaqX6a2QirYPI0yMPq_V1lpzr3m9vAUz3l3JdegVIYezA_apXPMa8DhDjZr1wIuYoaYmLgcS9fwyOqMRGe1z-3A5Whyt4nwYx7zVF8jSU9fspmBSFa2liCCQa7RjSUT8rCFKwBPUIbW8dNfRNQ5nhScLY5zXtYzQ3wtDcCv_P6Hrh4I8kzP4bCfTU2qTdjLG8gEDtf6W3mx4EV3fZGkIu_pDPz77eP5frFH56aU1Zn186T68CaE1qlo0tbJq0G6P6.whTGC8FCm3kbVE_4Lfyn6wnNtfj7_9YLwD8ET0AuXa8";
    public static async Task<T> GetResultParsedFromJson<T>(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer);
        var result = await client.SendAsync(message);
        // outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
        var serializer = JsonSerializer.Create();
        var parsed = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
        Assert.NotNull(parsed);
        return parsed;
    }
    
    public static async Task<T> GetResultParsedFromJson<T>(HttpClient client, string address, HttpMethod? method = null, ITestOutputHelper? outputHelper = null)
    {
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer);
        var result = await client.SendAsync(message);
        // outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
        var serializer = JsonSerializer.Create();
        var parsed = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
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
            // throw;
        }
    }
    
    public static async Task PostAsync(HttpClient client, string address, MultipartFormDataContent? form = null, ITestOutputHelper? outputHelper = null)
    {
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