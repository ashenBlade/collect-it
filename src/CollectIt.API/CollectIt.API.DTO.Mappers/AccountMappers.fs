module CollectIt.API.DTO.Mappers.AccountMappers

open System
open CollectIt.Database.Entities.Account
open CollectIt.API.DTO.AccountDTO
open CollectIt.Database.Entities.Account.Restrictions

let ToReadRestrictionDTO (restriction: Restriction) : ReadRestrictionDTO =
    match restriction with
    | null -> null
    | :? AuthorRestriction as author -> ReadAuthorRestrictionDTO(AuthorId = author.AuthorId)
    | :? DaysToRestriction as daysTo -> ReadDaysToRestrictionDTO(DaysTo = daysTo.DaysTo)
    | :? DaysAfterRestriction as daysAfter -> ReadDaysAfterRestrictionDTO(DaysAfter = daysAfter.DaysAfter)
    | :? TagRestriction as tag -> ReadTagsRestrictionDTO(Tags = tag.Tags)
    | _ -> failwith "Unsupported restriction type"

let ToReadUserDTO (user: User) (roles: string []) : ReadUserDTO =
    let dto = ReadUserDTO user.Id user.UserName user.Email roles
    dto

let ToReadSubscriptionDTO (subscription: Subscription) : ReadSubscriptionDTO =
    let dto =
        ReadSubscriptionDTO
            subscription.Id
            subscription.Name
            subscription.Description
            subscription.Price
            subscription.MonthDuration
            subscription.AppliedResourceType
            subscription.MaxResourcesCount
            subscription.Active
            (ToReadRestrictionDTO subscription.Restriction)

    dto

let ToReadRoleDTO (role: Role) : ReadRoleDTO =
    let dto = ReadRoleDTO role.Id role.Name

    dto


let ToReadUserSubscriptionDTO (userSubscription: UserSubscription) : ReadUserSubscriptionDTO =
    let dto =
        ReadUserSubscriptionDTO
            userSubscription.Id
            userSubscription.UserId
            userSubscription.SubscriptionId
            userSubscription.LeftResourcesCount
            (userSubscription.During.Start.ToDateTimeUnspecified())
            (userSubscription.During.End.ToDateTimeUnspecified())

    dto

let ToReadUserSubscriptionDTOFromActiveUserSubscription (aus: ActiveUserSubscription) : ReadUserSubscriptionDTO =
    let dto =
        ReadUserSubscriptionDTO
            aus.Id
            aus.UserId
            aus.SubscriptionId
            aus.LeftResourcesCount
            (aus.During.Start.ToDateTimeUnspecified())
            (aus.During.End.ToDateTimeUnspecified())

    dto

let ToRestrictionFromCreateRestrictionDTO (dto: CreateRestrictionDTO) =
    match dto with
    | :? CreateAuthorRestrictionDTO as author -> AuthorRestriction(AuthorId = author.AuthorId) :> Restriction
    | :? CreateDaysAfterRestrictionDTO as daysAfter -> DaysAfterRestriction(DaysAfter = daysAfter.DaysAfter)
    | :? CreateDaysToRestrictionDTO as daysTo -> DaysToRestriction(DaysTo = daysTo.DaysTo)
    | :? CreateTagsRestrictionDTO as tags -> TagRestriction(Tags = tags.Tags)
    | _ -> raise (ArgumentOutOfRangeException "Unsupported restriction type")


let ToReadAcquiredUserResourceDTO (aur: AcquiredUserResource) : ReadAcquiredUserResourceDTO =
    { PurchaseDate = aur.AcquiredDate
      Id = aur.ResourceId
      Name = aur.Resource.Name
    //       Address = aur.Resource.Address
    }
