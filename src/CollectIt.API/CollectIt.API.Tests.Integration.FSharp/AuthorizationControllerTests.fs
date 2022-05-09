module Tests

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Json
open System.Text.Json
open CollectIt.API.DTO
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions

[<Collection("OpenIddict tests")>]
type AuthorizationControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        interface IClassFixture<CollectItWebApplicationFactory>

        [<Fact>]
        member this.``Should get valid token with admin username and password``() =
            async {
                let admin =
                    PostgresqlCollectItDbContext.AdminUser

                use client = this._factory.CreateClient()

                let content =
                    new FormUrlEncodedContent(
                        [ KeyValuePair("grant_type", "password")
                          KeyValuePair("username", admin.UserName)
                          KeyValuePair("password", "12345678") ]
                    )

                use message =
                    new HttpRequestMessage(HttpMethod.Post, "/connect/token", Content = content)

                let log x = this.log (x.ToString())
                log 1

                let! response =
                    client.SendAsync(message, Async.DefaultCancellationToken)
                    |> Async.AwaitTask

                log 2

                let! result =
                    HttpContentJsonExtensions.ReadFromJsonAsync<OpenIddictResponseSuccess>(
                        response.Content,
                        JsonSerializerOptions(JsonSerializerDefaults.Web),
                        Async.DefaultCancellationToken
                    )
                    |> Async.AwaitTask

                log 3
                Assert.NotNull result
                Assert.Equal("Bearer", result.TokenType)
                Assert.NotEmpty result.AccessToken
            }
        //            |> Async.AwaitTask
//            |> Async.RunSynchronously

        [<Fact>]
        member this.``Register user with valid username, email and password should create new user``() =
            use client = this._factory.CreateClient()
            let username = "SomeValidUsername"
            let password = "SomeP@ssw0rd"
            let email = "test@mail.ru"

            let form =
                new FormUrlEncodedContent(
                    [ KeyValuePair("username", username)
                      KeyValuePair("password", password)
                      KeyValuePair("email", email) ]
                )

            let message =
                new HttpRequestMessage(HttpMethod.Post, "/connect/register", Content = form)

            task {

                let! response = client.SendAsync message
                Assert.Equal(HttpStatusCode.Created, response.StatusCode)
                let! actual = HttpContentJsonExtensions.ReadFromJsonAsync<AccountDTO.ReadUserDTO> response.Content
                Assert.NotNull actual
                Assert.Equal(username, actual.UserName)
                Assert.Equal(email, actual.Email)
            }



    end
