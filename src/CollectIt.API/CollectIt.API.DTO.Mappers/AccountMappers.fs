module CollectIt.API.DTO.Mappers.AccountMappers

open CollectIt.Database.Entities.Account
open CollectIt.API.DTO.AccountDTO

let ToReadUserDTO (user: User) (roles: string[]) : ReadUserDTO =
    let dto = ReadUserDTO user.Id user.UserName user.Email roles
    dto
    
let ToReadSubscriptionDTO (subscription: Subscription): ReadSubscriptionDTO =
    let dto = ReadSubscriptionDTO
                  subscription.Id
                  subscription.Name
                  subscription.Description
                  subscription.Price
                  subscription.MonthDuration
                  subscription.AppliedResourceType
                  subscription.MaxResourcesCount
                  subscription.RestrictionId
    dto
    
let ToReadRoleDTO (role: Role): ReadRoleDTO =
    let dto = ReadRoleDTO
                 role.Id
                 role.Name
                 
    dto
    