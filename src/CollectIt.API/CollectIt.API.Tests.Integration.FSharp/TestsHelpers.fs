module CollectIt.API.Tests.Integration.FSharp.TestsHelpers

open System.Collections.Generic
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Json
open CollectIt.API.Tests.Integration.FSharp.ConnectResult
open CollectIt.Database.Infrastructure
open Microsoft.FSharp.Control
open Newtonsoft.Json
open Xunit
open Xunit.Abstractions

let GetBearerForUserAsync
    (client: HttpClient)
    (username: string option)
    (password: string option)
    (helper: ITestOutputHelper option)
    =
    let username =
        match username with
        | Some u -> u
        | None -> PostgresqlCollectItDbContext.AdminUser.UserName

    let password =
        match password with
        | Some p -> p
        | None -> "12345678"

    let formContent =
        [ KeyValuePair("grant_type", "password")
          KeyValuePair("username", username)
          KeyValuePair("password", password) ]

    let message =
        new HttpRequestMessage(HttpMethod.Post, "connect/token", Content = new FormUrlEncodedContent(formContent))

    async {
        let! res = (client.SendAsync message |> Async.AwaitTask)

        let! value =
            HttpContentJsonExtensions.ReadFromJsonAsync<ConnectResult> res.Content
            |> Async.AwaitTask

        return value.Bearer
    }


let getResultParsedFromJson<'a>
    (client: HttpClient)
    (address: string)
    (bearer: string)
    (method: HttpMethod option)
    (outputHelper: ITestOutputHelper option)
    (content: HttpContent option)
    =
    let method =
        match method with
        | Some m -> m
        | None -> HttpMethod.Get

    let content =
        match content with
        | Some c -> c
        | None -> null

    use message = new HttpRequestMessage(method, address, Content = content)
    message.Headers.Authorization <- AuthenticationHeaderValue("Bearer", bearer)

    async {
        let! result = client.SendAsync message |> Async.AwaitTask

        let! json =
            result.Content.ReadAsStringAsync()
            |> Async.AwaitTask

        match outputHelper with
        | Some o -> o.WriteLine json
        | _ -> ()

        let parsed = JsonConvert.DeserializeObject<'a> json
        Assert.NotNull parsed
        return parsed
    }
