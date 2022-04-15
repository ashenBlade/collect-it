module CollectIt.API.DTO.Mappers.ResourcesMappers

open System.IO
open CollectIt.Database.Entities.Resources
open CollectIt.API.DTO.ResourcesDTO

let ToReadImageDTO (image: Image) : ReadImageDTO =
    let dto = ReadImageDTO image.OwnerId image.Name image.Tags image.Extension image.UploadDate
    dto