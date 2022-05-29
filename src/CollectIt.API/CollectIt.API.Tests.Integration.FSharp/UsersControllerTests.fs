module CollectIt.API.Tests.Integration.FSharp.UsersControllerTests

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Web
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Entities.Account
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions

[<Collection("Users")>]
type UsersControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        member this.NonexistentUserId
            with private get () =
                PostgresqlCollectItDbContext.DefaultUsers
                |> Array.maxBy (fun x -> x.Id)
                |> (fun u -> u.Id + 1000)




        interface IClassFixture<CollectItWebApplicationFactory>

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users; Return: json array of 4 initial users``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let! array =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO []> client $"api/v1/users" bearer None None None

                Assert.NotEmpty array
                let defaultUsersCount = 4
                Assert.Equal(defaultUsersCount, array.Length)
                client.Dispose()
            }

        member this.``Endpoint: GET /api/v1/users/{AdminUserId}; Return: json of admin user``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let expected =
                    PostgresqlCollectItDbContext.AdminUser

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"api/v1/users/{expected.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(expected.Id, actual.Id)
                Assert.Equal(expected.Email, actual.Email)
                Assert.Equal(expected.UserName, actual.UserName)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/{NonExistentUserId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonExistentUserId =
                    PostgresqlCollectItDbContext.DefaultUsers
                    |> Array.sumBy (fun u -> u.Id)
                    |> fun i -> i + 1

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{nonExistentUserId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/{UserId}/subscriptions; Return: json array of all subscriptions user purchased``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let! subscriptions =
                    TestsHelpers.getResultParsedFromJson<ReadUserSubscriptionDTO []>
                        client
                        $"/api/v1/users/{user.Id}/subscriptions"
                        bearer
                        None
                        None
                        None

                Assert.NotNull subscriptions
                Assert.NotEmpty subscriptions
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/{UserId}/active-subscriptions; Return: json array of current active subscriptions for user``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let! array =
                    TestsHelpers.getResultParsedFromJson<ReadUserSubscriptionDTO []>
                        client
                        $"/api/v1/users/{user.Id}/active-subscriptions"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty array
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/{UserId}/roles; Return: json array of roles for given user``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadRoleDTO []>
                        client
                        $"/api/v1/users/{user.Id}/roles"
                        bearer
                        None
                        None
                        None

                Assert.Contains(actual, (fun r -> r.Name = Role.AdminRoleName))
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/username; With: admin bearer; Should: change username``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let newUsername = "SomeNewUsername"

                let content =
                    new FormUrlEncodedContent([ KeyValuePair("username", newUsername) ])

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{user.Id}/username"
                        bearer
                        (Some content)
                        None
                        None
                        (Some HttpMethod.Post)

                response.EnsureSuccessStatusCode() |> ignore

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"/api/v1/users/{user.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newUsername, actual.UserName)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/username; With: invalid username (with whitespaces). Return: 400 BadRequest status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let invalidUsername =
                    "User name with whitespaces"

                let userId =
                    PostgresqlCollectItDbContext.DefaultUserTwoId

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/username"
                        bearer
                        HttpStatusCode.BadRequest
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("username", invalidUsername) ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/username; With: requester as same user. Should: change username``
            ()
            =
            task {
                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some user.UserName) (Some "12345678")

                let validUsername =
                    "SomeBrandNewValidUsername"

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{user.Id}/username"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("username", validUsername) ])))
                        None
                        None
                        (Some HttpMethod.Post)

                response.EnsureSuccessStatusCode() |> ignore

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"/api/v1/users/{user.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(validUsername, actual.UserName)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{NonExistingUserId}/username; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let userId =
                    PostgresqlCollectItDbContext.DefaultUsers
                    |> Array.sumBy (fun x -> x.Id)
                    |> fun x -> x + 1

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/email; Should: change user email``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let newEmail = "newemail@mail.ru"

                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{user.Id}/email"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("email", newEmail) ])))
                        None
                        None
                        (Some HttpMethod.Post)

                response.EnsureSuccessStatusCode() |> ignore

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"/api/v1/users/{user.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newEmail, actual.Email)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/email; With: invalid email. Return: 400 BadRequest status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let invalidEmail = "this is invalid email"

                let user =
                    PostgresqlCollectItDbContext.DefaultUserTwo

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{user.Id}/email"
                        bearer
                        HttpStatusCode.BadRequest
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("email", invalidEmail) ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/email; With: requester not admin or user itself; Return: 403 Forbidden status``
            ()
            =
            task {
                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let requester =
                    PostgresqlCollectItDbContext.DefaultUserTwo

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some requester.UserName) (Some "12345678")

                let email = "SomeValidEmail@mail.ru"

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{user.Id}/email"
                        bearer
                        HttpStatusCode.Forbidden
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("email", email) ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/roles; With: unassigned role; Should: assign role to user``
            ()
            =
            task {
                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let role = Role.TechSupportRoleName

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{user.Id}/roles"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("role_name", role) ])))
                        None
                        None
                        (Some HttpMethod.Post)

                ignore (response.EnsureSuccessStatusCode())

                let! roles =
                    TestsHelpers.getResultParsedFromJson<ReadRoleDTO []>
                        client
                        $"/api/v1/users/{user.Id}/roles"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty roles
                Assert.Contains(roles, (fun r -> r.Name = role))
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/users/{UserId}/roles; With: assigned role; Should: remove role from user``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.TechSupport

                let role = Role.TechSupportRoleName

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{user.Id}/roles"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("role_name", role) ])))
                        None
                        None
                        (Some HttpMethod.Delete)

                ignore (response.EnsureSuccessStatusCode())

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadRoleDTO []>
                        client
                        $"/api/v1/users/{user.Id}/roles"
                        bearer
                        None
                        None
                        None

                Assert.DoesNotContain(actual, (fun r -> r.Name = role))
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/users/{UserId}/roles; With: requester not in admin role; Return: 403 Forbidden status``
            ()
            =
            task {
                let requester =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some requester.UserName) (Some "12345678")

                let userId =
                    PostgresqlCollectItDbContext.TechSupportUserId

                let role = Role.TechSupportRoleName

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/roles"
                        bearer
                        HttpStatusCode.Unauthorized
                        (Some HttpMethod.Delete)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("role_name", role) ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/activate; With: deactivated user account; Return: 204 NoContent status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let userId =
                    PostgresqlCollectItDbContext.DefaultUserOneId

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{userId}/activate"
                        bearer
                        None
                        None
                        None
                        (Some HttpMethod.Post)

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{UserId}/deactivate; With: active user account; Return: 204 NoConten status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let userId =
                    PostgresqlCollectItDbContext.DefaultUserTwoId

                let! response =
                    TestsHelpers.sendAsync
                        client
                        $"/api/v1/users/{userId}/deactivate"
                        bearer
                        None
                        None
                        None
                        (Some HttpMethod.Post)

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/users/{NonExistingUserId}/roles. Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let userId =
                    PostgresqlCollectItDbContext.DefaultUsers
                    |> Array.maxBy (fun u -> u.Id)
                    |> (fun u -> u.Id + 1)

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users{userId}/roles"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Delete)
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{NonExistentUserId}/roles; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let userId = this.NonexistentUserId

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/roles"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("role_name", Role.AdminRoleName) ])))

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/users/{NonExistentUserId}/roles; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let userId = this.NonexistentUserId

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/roles"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Delete)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("role_name", Role.AdminRoleName) ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{NonExistingUserId}/activate; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let userId = this.NonexistentUserId

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/activate"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        None

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/users/{NonExistingUserId}/deactivate; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let userId = this.NonexistentUserId

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/{userId}/deactivate"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/with-username/{UserName}; Return: json of user with 200 OK status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.DefaultUserTwo

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"/api/v1/users/with-username/{HttpUtility.UrlEncode user.UserName}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(user.UserName, actual.UserName)
                Assert.Equal(user.Id, actual.Id)
                Assert.Equal(user.Email, actual.Email)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/with-username/{NonexistentUsername}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let username =
                    HttpUtility.UrlEncode $"{PostgresqlCollectItDbContext.DefaultUserOne}asdfasdf"

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/users/with-username/{username}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/with-email/{UserEmail}; Return: json of user``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadUserDTO>
                        client
                        $"api/v1/users/with-email/{HttpUtility.UrlEncode user.Email}"
                        bearer
                        None
                        None
                        None

                this._output.WriteLine $"{actual.UserName} {actual.Id} {actual.Email}"
                Assert.Equal(user.Id, actual.Id)
                Assert.Equal(user.UserName, actual.UserName)
                Assert.Equal(user.Email, actual.Email)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/users/with-email/{NonExistentEmail}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let email = "ThisEmailDoesNotExists@mail.ru"

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"api/v1/users/with-email/{HttpUtility.UrlEncode email}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }
    end
