module CollectIt.API.Tests.Integration.FSharp.MusicControllerTest

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

[<Collection("Music")>]
type MusicControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        static let assertTagsEqual (t1: IEnumerable<string>) (t2: IEnumerable<string>) : unit =
            let s1 = set t1
            let s2 = set t2
            let union = Set.union s2 s1
            Assert.Equal(s1.Count, union.Count)
            Assert.Equal(s2.Count, union.Count)

        static let assertMusicsEqual (dto1: ReadMusicDTO) (dto2: ReadMusicDTO) : unit =
            Assert.Equal(dto1.Name, dto2.Name)
            Assert.Equal(dto1.OwnerId, dto2.OwnerId)
            Assert.Equal(dto1.UploadDate, dto2.UploadDate)
            assertTagsEqual dto1.Tags dto2.Tags
            ()


        static let createMusicHttpContent (dto: CreateMusicDTO) : HttpContent =
            let content = new MultipartFormDataContent()
            content.Add(new StringContent(dto.Name), "Name")
            content.Add(new StringContent(dto.Extension), "Extension")
            content.Add(new StringContent(dto.Duration.ToString()), "Duration")
            Array.ForEach(dto.Tags, (fun t -> content.Add(new StringContent(t), "Tags")))

            let bytes =
                new ByteArrayContent(Array.Empty<byte>())

            bytes.Headers.ContentType <- Headers.MediaTypeHeaderValue($"audio/{dto.Extension}")
            content.Add(bytes, "Content", "SomeFileName.mp3")
            content

        static let toReadMusicDto (music: Music) : ReadMusicDTO =
            { Id = music.Id
              Name = music.Name
              UploadDate = music.UploadDate
              Extension = music.Extension
              Tags = music.Tags
              OwnerId = music.OwnerId
              Duration = music.Duration }

        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        member this.DefaultMusics
            with private get () = PostgresqlCollectItDbContext.DefaultMusics

        member this.DefaultMusic1
            with private get () = PostgresqlCollectItDbContext.DefaultMusic1

        member this.DefaultMusic2
            with private get () = PostgresqlCollectItDbContext.DefaultMusic2

        member this.DefaultMusic3
            with private get () = PostgresqlCollectItDbContext.DefaultMusic3

        interface IClassFixture<CollectItWebApplicationFactory>



        [<Fact>]
        member this.``Endpoint: GET /api/music?page_number=1&page_size=10; Return: json array of musics ordered by id``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let pageNumber = 1
                let pageSize = 10

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadMusicDTO []>
                        client
                        $"/api/music?page_number={pageNumber}&page_size={pageSize}"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty actual
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/music/{MusicId}; Return: json of required music``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let expected =
                    this.DefaultMusic1 |> toReadMusicDto

                let musicId = this.DefaultMusic1.Id

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadMusicDTO>
                        client
                        $"/api/music/{musicId}"
                        bearer
                        None
                        None
                        None

                assertMusicsEqual expected actual
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/music/{NonexistentMusicId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultMusics
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/music/{MusicId}/name; Should: change music name``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let music =
                    this.DefaultMusic2 |> toReadMusicDto

                let musicId = this.DefaultMusic2.Id

                let newName =
                    music.Name + " some string, but still valid"

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/music/{musicId}/name"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", newName) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadMusicDTO>
                        client
                        $"/api/music/{musicId}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newName, actual.Name)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/music/{NonexistentMusicId}/name; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentMusicId =
                    this.DefaultMusics
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun v -> v.Id + 100)

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{nonexistentMusicId}/name"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", "Some valid image name") ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/music/{MusicId}/tags; Should: change music's tags``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let musicId = this.DefaultMusic2.Id

                let expected =
                    [| "some"; "tags"; "from"; "f#" |]

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/music/{musicId}/tags"
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
                    TestsHelpers.getResultParsedFromJson<ReadMusicDTO>
                        client
                        $"/api/music/{musicId}"
                        bearer
                        None
                        None
                        None

                assertTagsEqual expected actual.Tags
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/music/{NonexistentMusicId}/tags; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultMusics
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{nonexistentId}/tags"
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
        member this.``Endpoint: DELETE /api/music/{MusicId}; Should: delete music``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let music = this.DefaultMusic3

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{music.Id}"
                        bearer
                        HttpStatusCode.NoContent
                        (Some HttpMethod.Delete)
                        None)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{music.Id}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }



        [<Fact>]
        member this.``Endpoint: DELETE /api/music/{NonexistentMusicId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultMusics
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Delete)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/music/{NonexistentMusicId}/download; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultMusics
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{nonexistentId}/download"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/music/{MusicId}/download; Return: content of file``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let musicId = this.DefaultMusic1.Id

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/music/{musicId}/download"
                        bearer
                        HttpStatusCode.OK
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/music; Should: create new music``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let music: CreateMusicDTO =
                    { Content = FormFile(Stream.Null, 0, 0, "SomeName", "FileName")
                      Duration = 10
                      Extension = "mp3"
                      Name = "Some music name"
                      Tags = [| "hello"; "best"; "dog" |] }

                let content = createMusicHttpContent music

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadMusicDTO>
                        client
                        $"/api/music"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        (Some content)

                Assert.NotNull actual
                Assert.Equal(music.Name, actual.Name)
                client.Dispose()
            }
    end
