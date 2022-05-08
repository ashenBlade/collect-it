module Tests

open System.Collections.Generic
open System.Net.Http
open System.Net.Http.Json
open CollectIt.API.DTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions

[<Collection("OpenIddict tests")>]
type AuthorizationControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output

        interface IClassFixture<CollectItWebApplicationFactory>

        [<Fact>]
        member this.``Should get valid token with admin username and password``() =
            let admin = PostgresqlCollectItDbContext.AdminUser
            use client = this._factory.CreateClient()

            let content =
                new FormUrlEncodedContent(
                    [ KeyValuePair("grant_type", "password")
                      KeyValuePair("username", admin.UserName)
                      KeyValuePair("password", "12345678") ]
                )

            use message =
                new HttpRequestMessage(HttpMethod.Post, "/connect/token", Content = content)

            task {
                let! response = client.SendAsync message

                let! result =
                    HttpContentJsonExtensions.ReadFromJsonAsync<AccountDTO.OpenIddictResponseSuccess> response.Content

                Assert.NotNull result
                Assert.Equal("Bearer", result.TokenType)
                Assert.NotEmpty result.AccessToken
            }
            |> Async.AwaitTask
            |> Async.RunSynchronously


    end
