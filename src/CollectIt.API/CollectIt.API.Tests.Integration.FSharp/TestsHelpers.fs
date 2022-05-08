module CollectIt.API.Tests.Integration.FSharp.TestsHelpers

open System.Net.Http
open CollectIt.Database.Infrastructure
open Xunit.Abstractions

let GetBearerForUserAsync (client: HttpClient) (username: string option) (password: string option) (helper: ITestOutputHelper option) =
//    let newUsername = match username with
//                            | Some u -> u
//                            | None -> PostgresqlCollectItDbContext.A
    ()