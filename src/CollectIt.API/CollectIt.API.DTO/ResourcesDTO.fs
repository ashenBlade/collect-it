module CollectIt.API.DTO.ResourcesDTO

open System
open System.ComponentModel.DataAnnotations
open System.IO
open CollectIt.Database.Entities.Resources
open Microsoft.AspNetCore.Http

[<CLIMutable>]
type ReadImageDTO = {
    [<Required>]
    OwnerId : int
    
    [<Required>]
    Name : string
    
    [<Required>]
    Tags : string[]
    
    [<Required>]
    Extension : string
    
    [<Required>]
    UploadDate : DateTime 
}

let ReadImageDTO ownerId name tags extension uploadDate = {
    OwnerId = ownerId
    Name = name
    Tags = tags
    Extension = extension
    UploadDate = uploadDate
}


[<CLIMutable>]
type ReadVideoDTO = {
    [<Required>]
    Id: int
    
    [<Required>]
    OwnerId : int
    
    [<Required>]
    Name : string
    
    [<Required>]
    Tags : string[]
    
    [<Required>]
    Extension : string
    
    [<Required>]
    UploadDate : DateTime
    
    [<Required>]
    Duration: int
}

let ReadVideoDTO id ownerId extension name tags duration uploadDate: ReadVideoDTO =
    let x: ReadVideoDTO = {
        Id = id
        OwnerId = ownerId
        Extension = extension
        Name = name
        Tags = tags
        Duration = duration
        UploadDate = uploadDate
    }
    x
    
[<CLIMutable>]
type CreateVideoDTO = {
    [<Required>]
    OwnerId : int
    
    [<Required>]
    Name : string
    
    [<Required>]
    Tags : string[]
    
    [<Required>]
    Extension : string
    
    [<Required>]
    UploadDate : DateTime
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    Duration: int
    
    [<Required>]
    Content: IFormFile
}