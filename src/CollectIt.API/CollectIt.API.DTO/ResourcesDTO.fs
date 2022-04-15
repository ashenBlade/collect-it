module CollectIt.API.DTO.ResourcesDTO

open System
open System.ComponentModel.DataAnnotations
open System.IO

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