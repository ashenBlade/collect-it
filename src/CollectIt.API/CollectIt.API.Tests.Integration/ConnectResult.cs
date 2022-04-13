using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CollectIt.API.Tests.Integration;

public class ConnectResult
{
    [JsonPropertyName("access_token")]
    public string Bearer { get; set; }

    [JsonPropertyName("expires_in")]
    public int Expires { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}