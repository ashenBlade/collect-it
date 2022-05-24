module CollectIt.API.DTO.Mappers.ResourcesMappers

open System.IO
open CollectIt.Database.Entities.Resources
open CollectIt.API.DTO.ResourcesDTO

let ToReadImageDTO (image: Image) : ReadImageDTO =
    let dto = ReadImageDTO image.OwnerId image.Name image.Tags image.Extension image.UploadDate
    dto
    
let ToReadMusicDTO (music: Music) : ReadMusicDTO =
    let dto = ReadMusicDTO music.OwnerId music.Name music.Tags music.Extension music.UploadDate music.Duration
    dto
    
let ToReadVideoDTO (v: Video) =
    let v: ReadVideoDTO = {
        Id = v.Id
        Duration = v.Duration
        Extension = v.Extension
        Name = v.Name
        UploadDate = v.UploadDate
        Tags = v.Tags
        OwnerId = v.OwnerId
    }
    v