module CollectIt.API.Tests.Integration.FSharp.StubMusicFileManager

open System.IO
open System.Threading.Tasks
open CollectIt.Database.Infrastructure.Resources.FileManagers

type StubMusicFileManager() =
    interface IMusicFileManager with
        member this.CreateAsync(filename, content) = Task.FromResult(FileInfo filename)
        member this.Delete(filename) = ()
        member this.GetContent(filename) = Stream.Null
