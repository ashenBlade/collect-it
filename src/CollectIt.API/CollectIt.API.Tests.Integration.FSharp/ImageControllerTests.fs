module CollectIt.API.Tests.Integration.FSharp.ImageControllerTests

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


[<Collection("Images")>]
type ImagesControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        static let assertTagsEqual (t1: IEnumerable<string>) (t2: IEnumerable<string>) : unit =
            let s1 = set t1
            let s2 = set t2
            let union = Set.union s2 s1
            Assert.Equal(s1.Count, union.Count)
            Assert.Equal(s2.Count, union.Count)

        static let assertImagesEqual (dto1: ReadImageDTO) (dto2: ReadImageDTO) : unit =
            Assert.Equal(dto1.Name, dto2.Name)
            Assert.Equal(dto1.OwnerId, dto2.OwnerId)
            Assert.Equal(dto1.UploadDate, dto2.UploadDate)
            assertTagsEqual dto1.Tags dto2.Tags
            ()


        static let createImageHttpContent (dto: CreateImageDTO) : HttpContent =
            let content = new MultipartFormDataContent()
            content.Add(new StringContent(dto.Name), "Name")
            content.Add(new StringContent(dto.Extension), "Extension")
            Array.ForEach(dto.Tags, (fun t -> content.Add(new StringContent(t), "Tags")))

            let bytes = new ByteArrayContent(Array.Empty<byte>())

            bytes.Headers.ContentType <- Headers.MediaTypeHeaderValue($"image/{dto.Extension}")
            content.Add(bytes, "Content", "SomeFileName.jpg")
            content

        static let toReadImageDto (img: Image) : ReadImageDTO =
            { Id = img.Id
              Name = img.Name
              UploadDate = img.UploadDate
              Extension = img.Extension
              Tags = img.Tags
              OwnerId = img.OwnerId }

        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        member this.DefaultImages
            with private get () = PostgresqlCollectItDbContext.DefaultImages

        member this.DefaultImage1
            with private get () = PostgresqlCollectItDbContext.DefaultImage1

        member this.DefaultImage2
            with private get () = PostgresqlCollectItDbContext.DefaultImage2

        member this.DefaultImage3
            with private get () = PostgresqlCollectItDbContext.DefaultImage3

        interface IClassFixture<CollectItWebApplicationFactory>



        [<Fact>]
        member this.``Endpoint: GET /api/images?page_number=1&page_size=10; Return: json array of images ordered by id``
            ()
            =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let pageNumber = 1
                let pageSize = 10

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadImageDTO []>
                        client
                        $"/api/images?page_number={pageNumber}&page_size={pageSize}"
                        bearer
                        None
                        None
                        None

                Assert.NotEmpty actual
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/images/{ImageId}; Return: json of required image``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let expected = this.DefaultImage1 |> toReadImageDto

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadImageDTO> client "/api/images/1" bearer None None None

                assertImagesEqual expected actual
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/images/{NonexistentImageId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultImages
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/images/{ImageId}/name; Should: change image name``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let image = this.DefaultImage2 |> toReadImageDto

                let imageId = 2

                let newName = image.Name + " some string, but still valid"

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/images/{imageId}/name"
                        bearer
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", newName) ])))
                        None
                        (Some true)
                        (Some HttpMethod.Post)
                     |> Async.AwaitTask
                     |> Async.Ignore)

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadImageDTO>
                        client
                        $"/api/images/{imageId}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(newName, actual.Name)
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/images/{NonexistentImageId}/name; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentImageId =
                    this.DefaultImages
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun v -> v.Id + 100)

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{nonexistentImageId}/name"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Post)
                        (Some(new FormUrlEncodedContent([ KeyValuePair("Name", "Some valid image name") ])))

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/images/{ImageId}/tags; Should: change image's tags``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let imageId = 2

                let expected = [| "some"; "tags"; "from"; "f#" |]

                do!
                    (TestsHelpers.sendAsync
                        client
                        $"/api/images/{imageId}/tags"
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
                    TestsHelpers.getResultParsedFromJson<ReadImageDTO>
                        client
                        $"/api/images/{imageId}"
                        bearer
                        None
                        None
                        None

                assertTagsEqual expected actual.Tags
                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: POST /api/images/{NonexistentImageId}/tags; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    (this.DefaultImages
                     |> Array.maxBy (fun v -> v.Id)
                     |> (fun v -> v.Id + 1000))

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{nonexistentId}/tags"
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
        member this.``Endpoint: DELETE /api/images/{ImageId}; Should: delete image``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let image = this.DefaultImage3

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{image.Id}"
                        bearer
                        HttpStatusCode.NoContent
                        (Some HttpMethod.Delete)
                        None)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{image.Id}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }



        [<Fact>]
        member this.``Endpoint: DELETE /api/images/{NonexistentImageId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultImages
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{nonexistentId}"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Delete)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/images/{NonexistentImageId}/download; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let nonexistentId =
                    this.DefaultImages
                    |> Array.maxBy (fun v -> v.Id)
                    |> (fun x -> x.Id + 100)

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{nonexistentId}/download"
                        bearer
                        HttpStatusCode.NotFound
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }

        [<Fact>]
        member this.``Endpoint: GET /api/images/{ImageId}/download; Return: content of file``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None
                let imageId = this.DefaultImage1.Id

                do!
                    (TestsHelpers.assertStatusCodeAsync
                        client
                        $"/api/images/{imageId}/download"
                        bearer
                        HttpStatusCode.OK
                        (Some HttpMethod.Get)
                        None)

                client.Dispose()
            }


        [<Fact>]
        member this.``Endpoint: POST /api/images; Should: create new image``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let image: CreateImageDTO =
                    { Content = FormFile(Stream.Null, 0, 0, "SomeName", "FileName")
                      Extension = "jpg"
                      Name = "Some image name"
                      Tags = [| "hello"; "best"; "dog" |] }

                let content = createImageHttpContent image

                let! actual =
                    TestsHelpers.getResultParsedFromJson<ReadImageDTO>
                        client
                        $"/api/images"
                        bearer
                        (Some HttpMethod.Post)
                        None
                        (Some content)

                Assert.NotNull actual
                Assert.Equal(image.Name, actual.Name)
                client.Dispose()
            }
    end
