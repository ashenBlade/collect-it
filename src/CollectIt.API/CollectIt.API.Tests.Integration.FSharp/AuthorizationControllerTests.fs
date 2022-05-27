module AuthorizationControllerTests

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
            task {
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

                let! response = client.SendAsync(message, Async.DefaultCancellationToken)

                let! result =
                    HttpContentJsonExtensions.ReadFromJsonAsync<OpenIddictResponseSuccess>(
                        response.Content,
                        JsonSerializerOptions(JsonSerializerDefaults.Web),
                        Async.DefaultCancellationToken
                    )

                Assert.NotNull result
                Assert.Equal("Bearer", result.TokenType)
                Assert.NotEmpty result.AccessToken
                client.Dispose()
            }

        [<Fact>]
        member this.``Register user with valid username, email and password should create new user``() =
            task {
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

                let! response = client.SendAsync message
                Assert.Equal(HttpStatusCode.Created, response.StatusCode)
                let! actual = HttpContentJsonExtensions.ReadFromJsonAsync<AccountDTO.ReadUserDTO> response.Content
                Assert.NotNull actual
                Assert.Equal(username, actual.UserName)
                Assert.Equal(email, actual.Email)
                client.Dispose()
            }

        [<Fact>]
        member this.``Register with already registered username - Should return BadRequest``() =
            task {
                use client = this._factory.CreateClient()

                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let dto: CreateUserDTO =
                    { UserName = user.UserName
                      Email = "oleg@mail.ru"
                      Password = "P@ssw0rd2H@rd" }

                let form =
                    new FormUrlEncodedContent(
                        [ KeyValuePair("username", dto.UserName)
                          KeyValuePair("email", dto.Email)
                          KeyValuePair("password", dto.Password) ]
                    )

                use message =
                    new HttpRequestMessage(HttpMethod.Post, "/connect/register", Content = form)

                let! response = client.SendAsync message
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode)
                client.Dispose()
            }

        [<Fact>]
        member this.``POST /connect/register; With incorrect password; Should return BadRequest``() =
            task {
                use client = this._factory.CreateClient()

                let createUserDTO: CreateUserDTO =
                    { UserName = "ValidUsername"
                      Email = "valid@email.com"
                      Password = System.String.Empty }

                let form =
                    new FormUrlEncodedContent(
                        [ KeyValuePair("username", createUserDTO.UserName)
                          KeyValuePair("email", createUserDTO.Email)
                          KeyValuePair("password", createUserDTO.Password) ]
                    )

                use message =
                    new HttpRequestMessage(HttpMethod.Post, "/connect/register", Content = form)

                let! response = client.SendAsync message
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode)
                client.Dispose()
            }
    end
