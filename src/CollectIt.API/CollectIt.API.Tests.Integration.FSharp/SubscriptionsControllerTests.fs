module CollectIt.API.Tests.Integration.FSharp.SubscriptionsControllerTests

open System.Collections.Generic
open System.Net
open System.Net.Http
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Entities.Account
open CollectIt.Database.Entities.Account.Restrictions
open CollectIt.Database.Infrastructure
open Xunit
open Xunit.Abstractions

[<Collection("Subscriptions")>]
type SubscriptionsControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
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

                let expected =
                    PostgresqlCollectItDbContext.SilverSubscription

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

        [<Fact>]
        member this.``Endpoint: GET /api/v1/subscriptions/active?type={ImageType}&page_number=1&page_size=10; Return: json array of active subscriptions``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let! parsed =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO []>
                        client
                        $"/api/v1/subscriptions/active?type={ResourceType.Image}&page_number=1&page_size=10"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty parsed
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions/{SubscriptionId}/name; With: valid name; Should: change name``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let newSubscriptionName =
                    "Some brand new subscription name"

                let subscription =
                    PostgresqlCollectItDbContext.BronzeSubscription

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/name"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("name", newSubscriptionName) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions/{subscription.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newSubscriptionName, actual.Name)

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/name"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("name", subscription.Name) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions/{SubscriptionID}/description; With: valid description: Should: change description``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let newSubscriptionDescription =
                    "Some brand new subscription description"

                let subscription =
                    PostgresqlCollectItDbContext.SilverSubscription

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/description"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("description", newSubscriptionDescription) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions/{subscription.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newSubscriptionDescription, actual.Description)

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/description"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("description", subscription.Description) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions/{SubscriptionId}/deactivate; With: active subscription; Should: change subscription state to deactivated``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let subscription =
                    PostgresqlCollectItDbContext.GoldenSubscription

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/deactivate"
                        bearer
                        None
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions/{subscription.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(false, actual.Active)

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/activate"
                        bearer
                        None
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions/{SubscriptionId}/activate; With: deactivated subscription; Should: change subscription state to activated``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let subscription =
                    PostgresqlCollectItDbContext.GoldenSubscription

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/activate"
                        bearer
                        None
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions/{subscription.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(true, actual.Active)

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/subscriptions/{subscription.Id}/deactivate"
                        bearer
                        None
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions; With: valid subscription creation data; Should: create new subscription``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let restriction: CreateRestrictionDTO =
                    CreateRestrictionDTO(RestrictionType.AllowAll)

                let dto: CreateSubscriptionDTO =
                    { Name = "Subscription name"
                      Description = "Simple description for subscription"
                      Price = 100
                      AppliedResourceType = ResourceType.Image
                      MonthDuration = 2
                      MaxResourcesCount = 100
                      Restriction = restriction }

                let content =
                    (new FormUrlEncodedContent(
                        [ KeyValuePair("Name", dto.Name)
                          KeyValuePair("Description", dto.Description)
                          KeyValuePair("Price", dto.Price.ToString())
                          KeyValuePair("AppliedResourceType", (int dto.AppliedResourceType).ToString())
                          KeyValuePair("MonthDuration", dto.MonthDuration.ToString())
                          KeyValuePair("MaxResourcesCount", dto.MaxResourcesCount.ToString()) ]
                    ))

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        (Some content)

                Assert.NotNull actual
                Assert.NotEqual(0, actual.Id)
                Assert.Equal(dto.Name, actual.Name)
                Assert.Equal(dto.Description, actual.Description)
                Assert.Equal(dto.Price, actual.Price)
                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/subscriptions; With: valid subscription creation data and restriction; Should: create new subscription``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let authorId =
                    PostgresqlCollectItDbContext.DefaultUserOneId

                let restriction: CreateAuthorRestrictionDTO =
                    CreateAuthorRestrictionDTO(authorId)

                let dto: CreateSubscriptionDTO =
                    { Name = "Subscription name"
                      Description = "Simple description for subscription"
                      Price = 100
                      AppliedResourceType = ResourceType.Image
                      MonthDuration = 2
                      MaxResourcesCount = 100
                      Restriction = restriction }

                let content =
                    (new FormUrlEncodedContent(
                        [ KeyValuePair("Name", dto.Name)
                          KeyValuePair("Description", dto.Description)
                          KeyValuePair("Price", dto.Price.ToString())
                          KeyValuePair("AppliedResourceType", (int dto.AppliedResourceType).ToString())
                          KeyValuePair("MonthDuration", dto.MonthDuration.ToString())
                          KeyValuePair("MaxResourcesCount", dto.MaxResourcesCount.ToString())
                          KeyValuePair("RestrictionType", (int restriction.RestrictionType).ToString())
                          KeyValuePair("AuthorId", restriction.AuthorId.ToString()) ]
                    ))

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadSubscriptionDTO>
                        client
                        $"/api/v1/subscriptions"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        (Some content)

                Assert.NotNull actual
                Assert.NotEqual(0, actual.Id)
                Assert.Equal(dto.Name, actual.Name)
                Assert.Equal(dto.Description, actual.Description)
                Assert.Equal(dto.Price, actual.Price)
                client.Dispose()
            }
    end
