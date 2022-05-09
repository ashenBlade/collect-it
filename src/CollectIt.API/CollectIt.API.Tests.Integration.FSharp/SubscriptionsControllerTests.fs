module CollectIt.API.Tests.Integration.FSharp.SubscriptionsControllerTests

open System.Net
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Entities.Account
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions


type RolesControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        member this.NonexistentId
            with private get () =
                [ PostgresqlCollectItDbContext.BronzeSubscription
                  PostgresqlCollectItDbContext.SilverSubscription
                  PostgresqlCollectItDbContext.GoldenSubscription
                  PostgresqlCollectItDbContext.DisabledSubscription
                  PostgresqlCollectItDbContext.AllowAllSubscription ]
                |> List.maxBy (fun s -> s.Id)
                |> (fun x -> x.Id + 10000)

        interface IClassFixture<CollectItWebApplicationFactory>

        [<Fact>]
        member this.``Endpoint: GET /api/v1/subscriptions?type=1&page_number=1&page_size=10; Return: json array of subscriptions for image``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO []>
                        client
                        $"/api/v1/subscriptions?type={ResourceType.Image}&page_number=1&page_size=10"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty actual
                client.Dispose()
            }



        [<Fact>]
        member this.``Endpoint: GET /api/v1/subscriptions/{SubscriptionId}; With: existing subscription id; Return: json of subscription``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let expected = PostgresqlCollectItDbContext.SilverSubscription

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions/{expected.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(expected.Id, actual.Id)
                Assert.Equal(expected.Name, actual.Name)
                Assert.Equal(expected.Description, actual.Description)
                Assert.Equal(expected.Price, actual.Price)
                Assert.Equal(expected.MonthDuration, actual.MonthDuration)
                Assert.Equal(expected.AppliedResourceType, actual.AppliedResourceType)
                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: GET /api/v1/subscriptions/{NonexistentId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/subscriptions/{this.NonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }
    end
