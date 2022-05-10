module CollectIt.API.Tests.Integration.FSharp.RolesControllerTests

open System.Net
open CollectIt.API.DTO.AccountDTO
open CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory
open CollectIt.Database.Entities.Account
open Xunit
open Xunit.Abstractions

[<Collection("Roles")>]
type RolesControllerTests(factory: CollectItWebApplicationFactory, output: ITestOutputHelper) =
    class
        member this._factory = factory
        member this._output = output
        member private this.log msg = this._output.WriteLine msg

        interface IClassFixture<CollectItWebApplicationFactory>


        [<Fact>]
        member this.``Endpoint: GET /api/v1/roles; Return: roles array``() =
            task {
                let! result = TestsHelpers.initialize this._factory None None

                let! roles =
                    TestsHelpers.getResultParsedFromJson<ReadRoleDTO []>
                        result.Client
                        "api/v1/roles"
                        result.Bearer
                        None
                        None
                        None

                Assert.NotEmpty roles
                Assert.Contains(roles, (fun r -> r.Name = Role.AdminRoleName))
            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/roles/{AdminRoleId}; Return: admin role json with 200 OK status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                let! role =
                    TestsHelpers.getResultParsedFromJson<ReadRoleDTO>
                        client
                        $"api/v1/roles/{Role.AdminRoleId}"
                        bearer
                        None
                        None
                        None

                Assert.Equal(Role.AdminRoleId, role.Id)
                Assert.Equal(Role.AdminRoleName, role.Name)

            }

        [<Fact>]
        member this.``Endpoint: GET /api/v1/roles/{NonExistingId}; Return: 404 NotFound status``() =
            task {
                let! { Bearer = bearer; Client = client } = TestsHelpers.initialize this._factory None None

                do!
                    TestsHelpers.assertStatusCodeAsync
                        client
                        "api/v1/roles/12435434213"
                        bearer
                        HttpStatusCode.NotFound
                        None
                        None
            }
    end
