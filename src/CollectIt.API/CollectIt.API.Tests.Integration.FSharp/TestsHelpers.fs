module CollectIt.API.Tests.Integration.FSharp.TestsHelpers

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Json
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.API.Tests.Integration.FSharp.ConnectResult
open CollectIt.Database.Infrastructure
open Microsoft.FSharp.Control
open Newtonsoft.Json
open Xunit
open Xunit.Abstractions

let getBearerForUserAsync
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


    task {
        use message =
            new HttpRequestMessage(HttpMethod.Post, "connect/token", Content = new FormUrlEncodedContent(formContent))

        let! res = client.SendAsync message

        let! value = HttpContentJsonExtensions.ReadFromJsonAsync<ConnectResult> res.Content

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

    task {
        use message =
            new HttpRequestMessage(method, address, Content = content)

        message.Headers.Authorization <- AuthenticationHeaderValue("Bearer", bearer)

        let! result = client.SendAsync message |> Async.AwaitTask

        let! json = result.Content.ReadAsStringAsync()

        match outputHelper with
        | Some o -> o.WriteLine json
        | _ -> ()

        let parsed =
            JsonConvert.DeserializeObject<'a> json

        Assert.NotNull parsed
        return parsed
    }

let unwrapOpt opt def =
    match opt with
    | Some s -> s
    | None -> def

let (??>) = unwrapOpt

let sendAsync
    (client: HttpClient)
    (address: string)
    (bearer: string)
    (content: HttpContent option)
    (output: ITestOutputHelper option)
    (ensure: bool option)
    (method: HttpMethod option)
    =
    task {
        use message =
            new HttpRequestMessage(method ??> HttpMethod.Get, address, Content = (content ??> null))

        message.Headers.Authorization <- AuthenticationHeaderValue("Bearer", bearer)

        let! response = client.SendAsync message

        if (ensure ??> false) then
            try
                response.EnsureSuccessStatusCode() |> ignore
            with
            | ex ->
                match output with
                | Some o ->
                    let! str = response.Content.ReadAsStringAsync()
                    o.WriteLine str
                | None -> ()

                raise ex

        return response
    }

let assertStatusCodeAsync
    (client: HttpClient)
    (address: string)
    (bearer: string)
    (expected: HttpStatusCode)
    (method: HttpMethod option)
    (content: HttpContent option)
    =

    task {
        use message =
            new HttpRequestMessage(method ??> HttpMethod.Get, address, Content = (content ??> null))

        message.Headers.Authorization <- AuthenticationHeaderValue("Bearer", bearer)
        let! response = client.SendAsync message
        let! res = response.Content.ReadAsStringAsync()
        Assert.Equal(expected, response.StatusCode)
    }

type InitializationResult = { Bearer: string; Client: HttpClient }

let initialize (factory: CollectItWebApplicationFactory) (username: string option) (password: string option) =
    task {
        let client = factory.CreateClient()
        let! bearer = getBearerForUserAsync client username password None
        return { Bearer = bearer; Client = client }
    }
