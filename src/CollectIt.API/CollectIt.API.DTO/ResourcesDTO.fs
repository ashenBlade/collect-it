module CollectIt.API.DTO.ResourcesDTO

open System
open System.ComponentModel.DataAnnotations
open Microsoft.AspNetCore.Http

[<CLIMutable>]
type ReadImageDTO =
    { [<Required>]
      Id: int

      [<Required>]
      OwnerId: int

      [<Required>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      Extension: string

      [<Required>]
      UploadDate: DateTime }


[<CLIMutable>]
type ReadMusicDTO =
    { [<Required>]
      Id: int

      [<Required>]
      OwnerId: int

      [<Required>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      Extension: string

      [<Required>]
      UploadDate: DateTime

      [<Required>]
      Duration: int }

let ReadImageDTO id ownerId name tags extension uploadDate =
    { Id = id
      OwnerId = ownerId
      Name = name
      Tags = tags
      Extension = extension
      UploadDate = uploadDate }

let ReadMusicDTO id ownerId name tags extension uploadDate duration =
    { Id = id
      OwnerId = ownerId
      Name = name
      Tags = tags
      Extension = extension
      UploadDate = uploadDate
      Duration = duration }



[<CLIMutable>]
type CreateImageDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(30)>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      [<MaxLength(10)>]
      Extension: string

      [<Required>]
      Content: IFormFile }

[<CLIMutable>]
type CreateMusicDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(30)>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      [<MaxLength(10)>]
      Extension: string

      [<Required>]
      Content: IFormFile

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      Duration: int }

[<CLIMutable>]
type ReadVideoDTO =
    { [<Required>]
      Id: int

      [<Required>]
      OwnerId: int

      [<Required>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      Extension: string

      [<Required>]
      UploadDate: DateTime

      [<Required>]
      Duration: int }

let ReadVideoDTO id ownerId extension name tags duration uploadDate : ReadVideoDTO =
    let x: ReadVideoDTO =
        { Id = id
          OwnerId = ownerId
          Extension = extension
          Name = name
          Tags = tags
          Duration = duration
          UploadDate = uploadDate }

    x

[<CLIMutable>]
type CreateVideoDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(30)>]
      Name: string

      [<Required>]
      Tags: string []

      [<Required>]
      [<MaxLength(10)>]
      Extension: string

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      Duration: int

      [<Required>]
      Content: IFormFile }
