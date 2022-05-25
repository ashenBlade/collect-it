module CollectIt.API.Tests.Integration.FSharp.StubImageFileManager

open System.IO
open System.Threading.Tasks
open CollectIt.Database.Infrastructure.Resources.FileManagers

type StubImageFileManager() =
    interface IImageFileManager with
        member this.CreateAsync(filename, content) = Task.FromResult(FileInfo filename)
        member this.Delete(filename) = ()
        member this.GetContent(filename) = Stream.Null
