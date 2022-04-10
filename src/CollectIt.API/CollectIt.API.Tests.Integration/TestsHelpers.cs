using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public static class TestsHelpers
{
    public static async Task<T> GetResultParsedFromJson<T>(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        var result = await client.SendAsync(message);
        outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
        var serializer = JsonSerializer.Create();
        var parsed = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
        Assert.NotNull(parsed);
        return parsed;
    }

    public static async Task AssertNotFoundAsync(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        var result = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}