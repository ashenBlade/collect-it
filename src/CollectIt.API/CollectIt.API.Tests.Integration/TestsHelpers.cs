using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public static class TestsHelpers
{
    public static string AdminAccessTokenBearer =>
        "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIiLCJraWQiOiJEN0E1NDZFNzFDRDA0MkI3MzkyMzVCOTU0MjBDOUQ1OEFGRDE4QUZCIiwidHlwIjoiYXQrand0In0.XBa7Gd39A2ekFnbE2pvueDhFmo2nlb82WXE0XWPdbr3ZOzefAqOEOBTO2-sXvRWppXMYCgSSt7GDpgi-tOcrhzk8XkmbJcfWB1kNvGA254lM2IQ4xkfAQAvtA_T_dlA7sVzKJ76koHM2pEUdXIEVWUVolEb39c1UZblRrxTO70aOy0G0RhemlkIpqyxp0IWnkUXBv8WlYgiipka8yGwZK2KhpE-1UQ7tHB97CmnR45a9WB_WIWKrsQZUMTUvHA5hyqSO1kpMpn-wwrEwkNxa6pqqls_i_aZy3sKWdlPzNF9jTJ2BntPKLi6R7kYI9ahtjyI_PwFS2Cwl_VgHKuaddA.UfUnFbq2ozvv__hbQLh6_w.gqURk6I0NZkh2hgn3UvONHp_q9jBUiI6CrQClKDO0wns1EfvfZSfinJJDCI3rWjXauA44d8lCNCH_9Rq1AqOIR-Mt2P2JZAl0JyUHOVk9K_X0rF5eQks2ymXn1XRiSOsFaKN-RL2BDrt7BjX913EHXWGOUrsyyt_M4wZ96JXxZUSJTiPOWzzxgNH1eSJE3WxRlwaaFrIkqJEGca-oycPvxtkRGayIt4nRiJMhfzUfMoeZOjuJBxAgg4XztHe36zq2rtAQdRdnh-hg9nw8wM3Ep8lRbUCBPK5sU7McQpHPo7Rn9SdMC37V92b3jnNJwf7mtbab_XMP0Ue6GgpARiMcdz_hIUgqL2b8TqpSd2Y1CNF4IrRMsYpxjgbkZO8NHHSgXxASMvmcolz0xRPHz-hZk7KtTmy9G64y5-2HZVwIQSSgSc8qXa_LWXPFgORrypc_fMUeYNVNivpG0HeA4gynnD1bGoeP9Vbxx6Wjs3dqkRSS6T6A3E_N7RZJqpLn6-P6Ynpe4Sgv0j6FppdQLna4jnSi31MUABVx7oBQCs-NbnFDQApJyzaTBU7YJ3rCmpNl14Fa32a4T-zbfn5CM2W7AAVwZ8mz7f_ArNq5wcmGwGurTbITDP1fAEh20A_sGvhV7qWKjpB8tstt2IQvHmVLfHf5Sdj1GELFATm_5s54YeIPVV8Wm60ggkDGYbaP2pkcdxMgIGc434jQ3epDSYlwQIBkSB3kGo_kK5rLNVrSUwqSv-oxx0N00PqEeVpXgwQFutsQKqPaLrj0Sm9U0SRfLilgDwVL9JGmV0_rOoiureWhzeE8GKnqoV2DiXyJNqWscoGanRmgRh6aT57ayU7sF8VHQUzdNoqDP3MIZS9_Zbks0uHG8GRShEVBGSPRRTMcEwO08zJwd4FOUv69Wqo8A.zgC7Ft7YpIsRI9EYHEbT5FywxtN2KfF58jiW8qUqxqs";
    public static async Task<T> GetResultParsedFromJson<T>(CollectItWebApplicationFactory factory, string address, HttpMethod? method = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminAccessTokenBearer);
        var result = await client.SendAsync(message);
        outputHelper?.WriteLine(await result.Content.ReadAsStringAsync());
        var serializer = JsonSerializer.Create();
        var parsed = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
        Assert.NotNull(parsed);
        return parsed;
    }

    public static async Task PostAsync(CollectItWebApplicationFactory factory, string address, object? toSend = null, ITestOutputHelper? outputHelper = null)
    {
        using var client = factory.CreateClient();
        var result = await client.PostAsync(address, new MultipartContent());
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