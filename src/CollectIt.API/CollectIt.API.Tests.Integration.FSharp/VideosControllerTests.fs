module CollectIt.API.Tests.Integration.FSharp.VideosControllerTests

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Net.Http
open CollectIt.API.DTO.ResourcesDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Entities.Resources
open CollectIt.Database.Infrastructure
open Microsoft.AspNetCore.Http
open Xunit
open Xunit.Abstractions

type x = (IEnumerable<string> * IEnumerable<string>) -> unit

[<Collection("Videos")>]
type VideosControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        static let assertTagsEqual (t1: IEnumerable<string>) (t2: IEnumerable<string>) : unit =
            let s1 = set t1
            let s2 = set t2
            let union = Set.union s2 s1
            Assert.Equal(s1.Count, union.Count)
            Assert.Equal(s2.Count, union.Count)

        static let assertVideosEqual (dto1: ReadVideoDTO) (dto2: ReadVideoDTO) : unit =
            Assert.Equal(dto1.Id, dto2.Id)
            Assert.Equal(dto1.Name, dto2.Name)
            Assert.Equal(dto1.OwnerId, dto2.OwnerId)
            Assert.Equal(dto1.UploadDate, dto2.UploadDate)
            assertTagsEqual dto1.Tags dto2.Tags
            ()

        static let createVideoHttpContent (dto: CreateVideoDTO) : HttpContent =
            let content = new MultipartFormDataContent()
            content.Add(new StringContent(dto.Name), "Name")
            content.Add(new StringContent(dto.Extension), "Extension")
            content.Add(new StringContent(dto.Duration.ToString()), "Duration")
            Array.ForEach(dto.Tags, (fun t -> content.Add(new StringContent(t), "Tags")))

            let bytes =
                new ByteArrayContent(Array.Empty<byte>())

            bytes.Headers.ContentType <- Headers.MediaTypeHeaderValue($"video/{dto.Extension}")
            content.Add(bytes, "Content", "SomeFileName.webp")
            content

        static let toReadVideoDto (v: Video) : ReadVideoDTO =
            { Id = v.Id
              Name = v.Name
              UploadDate = v.UploadDate
              Extension = v.Extension
              Duration = v.Duration
              Tags = v.Tags
              OwnerId = v.OwnerId }

        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        member this.DefaultVideos
            with private get () = PostgresqlCollectItDbContext.DefaultVideos

        member this.DefaultVideo1
            with private get () = PostgresqlCollectItDbContext.DefaultVideo1

        member this.DefaultVideo2
            with private get () = PostgresqlCollectItDbContext.DefaultVideo2

        member this.DefaultVideo3
            with private get () = PostgresqlCollectItDbContext.DefaultVideo3

        interface IClassFixture<CollectItWebApplicationFactory>


        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos?&page_number=1&page_size=10; Return: json array of videos ordered by id``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let pageNumber = 1
                let pageSize = 5

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadVideoDTO []>
                        client
                        $"/api/v1/videos?page_number={pageNumber}&page_size={pageSize}"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty actual
                client.Dispose()
            }


        //        [<Fact>]
//        member this.``Endpoint: GET /api/v1/videos/; Return: json of required video`` () =
//            task {
//                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
//
//                client.Dispose()
//            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos/{VideoId}; Return: json of required video``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let expected =
                    this.DefaultVideo1 |> toReadVideoDto

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadVideoDTO>
                        client
                        $"/api/v1/videos/{expected.Id}"
                        bearer
                        None
                        None
                        None

                assertVideosEqual expected actual
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos/{NonexistentVideoId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultVideos
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/videos/{VideoId}/name; Should: change video name``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let video =
                    this.DefaultVideo2 |> toReadVideoDto

                let newName =
                    video.Name + " some string, but still valid"

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/videos/{video.Id}/name"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", newName) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadVideoDTO>
                        client
                        $"/api/v1/videos/{video.Id}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newName, actual.Name)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/videos/{NonexistentVideoId}/name; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentVideoId =
                    this.DefaultVideos
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun v -> v.Id + 100)

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{nonexistentVideoId}/name"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", "Some valid video name") ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/videos/{VideoId}/tags; Should: change videos tags``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let video =
                    this.DefaultVideo2 |> toReadVideoDto

                let expected =
                    [| "some"; "tags"; "from"; "f#" |]

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/v1/videos/{video.Id}/tags"
                        bearer
                        (Some(
                            new FormUrlEncodedContent(
                                expected
                                |> Array.map (fun t -> KeyValuePair("Tags", t))
                            )
                        ))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadVideoDTO>
                        client
                        $"/api/v1/videos/{video.Id}"
                        bearer
                        None
                        None
                        None

                assertTagsEqual expected actual.Tags
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/v1/videos/{NonexistentVideoId}/tags; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultVideos
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{nonexistentId}/tags"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        (Some(
                            new FormUrlEncodedContent(
                                [ KeyValuePair("Tags", "tag")
                                  KeyValuePair("Tags", "new")
                                  KeyValuePair("Tags", "best") ]
                            )
                        ))

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/v1/videos; Should: create new video``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let video: CreateVideoDTO =
                    { Content = FormFile(Stream.Null, 0, 0, "SomeName", "FileName")
                      Duration = 10
                      Extension = "webm"
                      Name = "Some video name"
                      Tags = [| "hello"; "best"; "dog" |] }

                let content = createVideoHttpContent video

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadVideoDTO>
                        client
                        $"/api/v1/videos"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        (Some content)

                Assert.NotNull actual
                Assert.NotEqual(0, actual.Id)
                Assert.Equal(video.Name, actual.Name)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/videos/{VideoId}; Should: delete video``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let video = this.DefaultVideo3

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{video.Id}"
                        bearer
                        HttpStatusCode.NoContent
                        (Some HttpMethod.Delete)
                        None)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{video.Id}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: DELETE /api/v1/videos/{NonexistentVideoId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultVideos
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Delete)
                        None)

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos/{NonexistentVideoId}/download; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultVideos
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{nonexistentId}/download"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos/{VideoId}/download; Return: content of file``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let videoId = this.DefaultVideo1.Id

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{videoId}/download"
                        bearer
                        HttpStatusCode.OK
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/videos/{VideoId}/download; With: requester did not acquired video; Return: 402 PaymentRequired status``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } =
                    TestsHelpers.initialize
                        this._factory
                        (Some PostgresqlCollectItDbContext.DefaultUserOne.UserName)
                        None

                let video = this.DefaultVideo1

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/v1/videos/{video.Id}/download"
                        bearer
                        HttpStatusCode.PaymentRequired
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }
    end
