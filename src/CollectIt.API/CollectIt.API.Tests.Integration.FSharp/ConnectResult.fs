module CollectIt.API.Tests.Integration.FSharp.ConnectResult

open System.Text.Json.Serialization

[<CLIMutable>]
type ConnectResult =
    { [<JsonPropertyName("access_token")>]
      Bearer: string
      [<JsonPropertyName("expires_in")>]
      Expires: int
      [<JsonPropertyName("token_type")>]
      TokenType: string }
