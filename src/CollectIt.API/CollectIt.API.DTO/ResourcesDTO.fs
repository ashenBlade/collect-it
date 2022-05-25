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


[<CLIMutable>]
type ReadMusicDTO = {
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

let ReadImageDTO ownerId name tags extension uploadDate = {
    OwnerId = ownerId
    Name = name
    Tags = tags
    Extension = extension
    UploadDate = uploadDate
}

let ReadMusicDTO ownerId name tags extension uploadDate duration = {
    OwnerId = ownerId
    Name = name
    Tags = tags
    Extension = extension
    UploadDate = uploadDate
    Duration = duration
}



[<CLIMutable>]
type CreateImageDTO = {
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
    Content: IFormFile
}

[<CLIMutable>]
type CreateMusicDTO = {
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
    Content: IFormFile
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    Duration: int
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