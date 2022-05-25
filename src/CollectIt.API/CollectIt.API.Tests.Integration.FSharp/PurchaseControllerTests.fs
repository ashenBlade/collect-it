module CollectIt.API.Tests.Integration.FSharp.PurchaseControllerTests

open System.Net
open System.Net.Http
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions

[<Collection("Purchase")>]
type PurchaseControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        interface IClassFixture<CollectItWebApplicationFactory>

        [<Fact>]
        member this.``Endpoint: POST /api/v1/purchase/subscription/{SubscriptionId}; Should: subscribe requester``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let subscription =
                    PostgresqlCollectItDbContext.SilverSubscription

                let! result =
                    TestsHelpers.getResultParsedFromJson<ReadUserSubscriptionDTO>
                        client
                        $"/api/v1/purchase/subscription/{subscription.Id}"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        None

                Assert.Equal(subscription.Id, result.SubscriptionId)
                Assert.True(result.DateFrom < result.DateTo)
                Assert.True(0 < result.LeftResourcesCount)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/purchase/subscription/{NonexistentSubscriptionId}; Return: 404 NotFound status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let subscriptionId = 1000

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/purchase/subscription/{subscriptionId}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/purchase/subscription/{SubscriptionId}; With: already purchased subscription; Return: 400 BadRequest status``
            ()
            =
            task {
                let userSubscription =
                    PostgresqlCollectItDbContext.DefaultUserOneGoldenSubscriptionUserSubscription

                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some user.UserName) None

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/purchase/subscription/{userSubscription.SubscriptionId}"
                        bearer
                        HttpStatusCode.BadRequest
                        (Some HttpMethod.Post)
                        None

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/purchase/image/{ImageId}; Should: purchase required image``() =
            task {
                let user =
                    PostgresqlCollectItDbContext.DefaultUserOne

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some user.UserName) None

                let image =
                    PostgresqlCollectItDbContext.DefaultImage1
                //                    PostgresqlCollectItDbContext.DefaultImages
//                    |> Seq.head

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/purchase/image/{image.Id}"
                        bearer
                        None
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! acquired =
                    TestsHelpers.getResultParsedFromJson<ReadAcquiredUserResourceDTO []>
                        client
                        $"api/v1/users/{user.Id}/images"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty acquired
                Assert.Contains(acquired, (fun aur -> aur.Id = image.Id))
                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/purchase/image/{ImageId}; With: already acquired image; Return: 400 BadRequest status``
            ()
            =
            task {
                let user =
                    PostgresqlCollectItDbContext.AdminUser

                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize this._factory (Some user.UserName) None

                let image =
                    PostgresqlCollectItDbContext.DefaultImages
                    |> Seq.head

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/purchase/image/{image.Id}"
                        bearer
                        HttpStatusCode.BadRequest
                        (Some HttpMethod.Post)
                        None

                client.Dispose()
            }
    end
