module CollectIt.API.DTO.Mappers.ResourcesMappers

open System.IO
open CollectIt.Database.Entities.Resources
open CollectIt.API.DTO.ResourcesDTO

let ToReadImageDTO (image: Image) (fileBytes : byte[]) : ReadImageDTO =
    let dto = ReadImageDTO image.Owner.Id image.Name image.Tags image.Extension image.UploadDate fileBytes
    dto