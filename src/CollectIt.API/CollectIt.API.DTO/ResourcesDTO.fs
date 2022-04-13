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
    
    [<Required>]
    Image : byte[]
    
}

let ReadImageDTO ownerId name tags extension uploadDate fileBytes = {
    OwnerId = ownerId
    Name = name
    Tags = tags
    Extension = extension
    UploadDate = uploadDate
    Image = fileBytes
} 