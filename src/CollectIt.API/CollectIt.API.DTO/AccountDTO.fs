module CollectIt.API.DTO.AccountDTO

open System
open System.ComponentModel.DataAnnotations
open System.Text.Json.Serialization
open CollectIt.Database.Entities.Account
open CollectIt.Database.Entities.Account.Restrictions

[<CLIMutable>]
type CreateUserDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(20)>]
      UserName: string

      [<Required>]
      [<DataType(DataType.EmailAddress)>]
      Email: string

      [<Required>]
      [<DataType(DataType.Password)>]
      [<MinLength(6)>]
      [<MaxLength(20)>]
      Password: string }

let CreateUserDTO username email password =
    { UserName = username
      Email = email
      Password = password }

[<CLIMutable>]
type ReadUserDTO =
    { [<Required>]
      Id: int

      [<Required>]
      UserName: string

      [<Required>]
      [<DataType(DataType.EmailAddress)>]
      Email: string

      [<Required>]
      Roles: string [] }

let ReadUserDTO id username email roles =
    { Id = id
      UserName = username
      Email = email
      Roles = roles }


type CreateRestrictionDTO =
    val mutable private restrictionType: RestrictionType

    member this.RestrictionType
        with public get () = this.restrictionType
        and public set value = this.restrictionType <- value

    new(restrictionType: RestrictionType) = { restrictionType = restrictionType }


type CreateAuthorRestrictionDTO =
    inherit CreateRestrictionDTO

    val mutable private authorId: int

    member this.AuthorId
        with get () = this.authorId
        and set value = this.authorId <- value

    new(authorId: int) =
        { inherit CreateRestrictionDTO(RestrictionType.Author)
          authorId = authorId }

type CreateDaysToRestrictionDTO =
    inherit CreateRestrictionDTO

    val mutable private daysTo: int

    member this.DaysTo
        with get () = this.daysTo
        and set value = this.daysTo <- value

    new(daysTo: int) =
        { inherit CreateRestrictionDTO(RestrictionType.DaysTo)
          daysTo = daysTo }

type CreateDaysAfterRestrictionDTO =
    inherit CreateRestrictionDTO

    val mutable private daysAfter: int

    member this.DaysAfter
        with get () = this.daysAfter
        and set value = this.daysAfter <- value

    new(daysAfter: int) =
        { inherit CreateRestrictionDTO(RestrictionType.DaysAfter)
          daysAfter = daysAfter }

type CreateTagsRestrictionDTO =
    inherit CreateRestrictionDTO

    val mutable private tags: string []

    member this.Tags
        with get () = this.tags
        and set value = this.tags <- value

    new(tags: string []) =
        { inherit CreateRestrictionDTO(RestrictionType.Tags)
          tags = tags }

[<CLIMutable>]
type CreateSubscriptionDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(20)>]
      Name: string

      [<Required>]
      [<MinLength(10)>]
      [<MaxLength(50)>]
      Description: string

      [<Required>]
      [<Range(0, Int32.MaxValue)>]
      Price: int

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      MonthDuration: int

      [<Required>]
      AppliedResourceType: ResourceType

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      MaxResourcesCount: int

      Restriction: CreateRestrictionDTO }

let CreateSubscriptionDTO name description price monthDuration resourceType maxResourcesCount restriction =
    { Name = name
      Description = description
      Price = price
      MonthDuration = monthDuration
      AppliedResourceType = resourceType
      MaxResourcesCount = maxResourcesCount
      Restriction = restriction }

[<AllowNullLiteral>]
type ReadRestrictionDTO =
    val mutable private restrictionType: string

    member this.RestrictionType
        with get () = this.restrictionType
        and set value = this.restrictionType <- value

    new() = { restrictionType = "" }

type ReadAuthorRestrictionDTO =
    inherit ReadRestrictionDTO
    val mutable private authorId: int

    member this.AuthorId
        with public get () = this.authorId
        and public set value = this.authorId <- value

    new() =
        { inherit ReadRestrictionDTO(RestrictionType = "Author")
          authorId = 0 }



type ReadDaysAfterRestrictionDTO =
    inherit ReadRestrictionDTO
    val mutable private daysAfter: int

    member this.DaysAfter
        with get () = this.daysAfter
        and set value = this.daysAfter <- value

    new() =
        { inherit ReadRestrictionDTO(RestrictionType = "DaysAfter")
          daysAfter = 0 }


type ReadDaysToRestrictionDTO =
    inherit ReadRestrictionDTO
    val mutable private daysTo: int

    member this.DaysTo
        with get () = this.daysTo
        and set value = this.daysTo <- value

    new() =
        { inherit ReadRestrictionDTO(RestrictionType = "DaysTo")
          daysTo = 0 }

type ReadTagsRestrictionDTO =
    inherit ReadRestrictionDTO
    val mutable private tags: string []

    member this.Tags
        with get () = this.tags
        and set value = this.tags <- value

    new() =
        { inherit ReadRestrictionDTO(RestrictionType = "Tags")
          tags = [||] }

[<CLIMutable>]
type ReadSubscriptionDTO =
    { [<Required>]
      Id: int

      [<Required>]
      Name: string

      [<Required>]
      Description: string

      [<Required>]
      [<Range(0, Int32.MaxValue)>]
      Price: int

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      MonthDuration: int

      [<Required>]
      AppliedResourceType: ResourceType

      [<Required>]
      [<Range(1, Int32.MaxValue)>]
      MaxResourcesCount: int

      [<Required>]
      Active: bool

      Restriction: ReadRestrictionDTO }

let ReadSubscriptionDTO
    id
    name
    description
    price
    monthDuration
    appliedResourceType
    maxResourcesCount
    active
    restriction
    =
    { Id = id
      Name = name
      Description = description
      Price = price
      MonthDuration = monthDuration
      AppliedResourceType = appliedResourceType
      MaxResourcesCount = maxResourcesCount
      Restriction = restriction
      Active = active }

[<CLIMutable>]
type ReadUserSubscriptionDTO =
    { [<Required>]
      Id: int

      [<Required>]
      UserId: int

      [<Required>]
      SubscriptionId: int

      [<Required>]
      LeftResourcesCount: int

      [<Required>]
      DateFrom: DateTime

      [<Required>]
      DateTo: DateTime }

let ReadUserSubscriptionDTO id userId subscriptionId leftResourcesCount dateFrom dateTo =
    { Id = id
      UserId = userId
      SubscriptionId = subscriptionId
      LeftResourcesCount = leftResourcesCount
      DateFrom = dateFrom
      DateTo = dateTo }

[<CLIMutable>]
type CreateUserSubscriptionDTO =
    { [<Required>]
      UserId: int

      [<Required>]
      SubscriptionId: int }

let CreateUserSubscriptionDTO userId subscriptionId =
    { UserId = userId
      SubscriptionId = subscriptionId }

[<CLIMutable>]
type ReadRoleDTO =
    { [<Required>]
      Id: int

      [<Required>]
      Name: string }

let ReadRoleDTO id name = { Id = id; Name = name }

[<CLIMutable>]
type ReadActiveUserSubscription =
    { [<Required>]
      Id: int

      [<Required>]
      UserId: int

      [<Required>]
      SubscriptionId: int

      [<Required>]
      [<Range(0, Int32.MaxValue)>]
      LeftResourcesCount: int

      [<Required>]
      [<DataType(DataType.Date)>]
      DateFrom: DateTime

      [<Required>]
      [<DataType(DataType.Date)>]
      DateTo: DateTime }

let ReadActiveUserSubscription
    id
    userId
    subscriptionId
    leftResourcesCount
    dateFrom
    dateTo
    : ReadActiveUserSubscription =
    { Id = id
      UserId = userId
      SubscriptionId = subscriptionId
      LeftResourcesCount = leftResourcesCount
      DateFrom = dateFrom
      DateTo = dateTo }

[<CLIMutable>]
type SearchSubscriptionsDTO =
    { [<Required>]
      ResourceType: ResourceType
      Name: string }

[<CLIMutable>]
type OpenIddictResponseSuccess =
    { [<JsonPropertyName("access_token")>]
      AccessToken: string

      [<JsonPropertyName("token_type")>]
      TokenType: string

      [<JsonPropertyName("expires_in")>]
      ExpiresIn: int }

[<CLIMutable>]
type OpenIddictResponseFail = { NONE: int }


[<CLIMutable>]
type ReadAcquiredUserResourceDTO =
    { PurchaseDate: DateTime
      Id: int
      //  Address: string
      Name: string }

[<CLIMutable>]
type RegisterDTO =
    { [<Required>]
      [<MinLength(6)>]
      [<MaxLength(30)>]
      Username: string

      [<Required>]
      [<MinLength(6)>]
      [<MaxLength(50)>]
      Password: string

      [<Required>]
      [<EmailAddress>]
      [<MaxLength(50)>]
      Email: string }
