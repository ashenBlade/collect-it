module CollectIt.API.DTO.AccountDTO

open System
open System.ComponentModel.DataAnnotations
open CollectIt.Database.Entities.Account

[<CLIMutable>]
type CreateUserDTO = {
    [<Required>]
    UserName : string
    
    [<Required>]
    [<DataType(DataType.EmailAddress)>]
    Email : string
    
    [<Required>]
    [<DataType(DataType.Password)>]
    Password : string
}

let CreateUserDTO username email password = {
    UserName = username
    Email = email
    Password = password
} 

[<CLIMutable>]
type ReadUserDTO = {
    [<Required>]
    Id : int
    
    [<Required>]
    UserName : string
    
    [<Required>]
    [<DataType(DataType.EmailAddress)>]
    Email : string
    
    [<Required>]
    Roles : string[]
    
}

let ReadUserDTO id username email roles = {
    Id = id
    UserName = username
    Email = email
    Roles = roles
} 


[<CLIMutable>]
type CreateSubscriptionDTO = {
    [<Required>]
    Name : string
    
    [<Required>]
    Description : string
    
    [<Required>]
    [<Range(0, Int32.MaxValue)>]
    Price : int
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    MonthDuration : int
    
    [<Required>]
    AppliedResourceType : ResourceType
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    MaxResourcesCount : int
    
    RestrictionId : Nullable<int>
}

let CreateSubscriptionDTO name description price monthDuration resourceType maxResourcesCount restrictionId = {
    Name = name
    Description = description
    Price = price
    MonthDuration = monthDuration
    AppliedResourceType = resourceType
    MaxResourcesCount = maxResourcesCount
    RestrictionId = restrictionId
}

[<CLIMutable>]
type ReadSubscriptionDTO = {
    [<Required>]
    Id : int
    
    [<Required>]
    Name : string
    
    [<Required>]
    Description : string
    
    [<Required>]
    [<Range(0, Int32.MaxValue)>]
    Price : int
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    MonthDuration : int
    
    [<Required>]
    AppliedResourceType : ResourceType
    
    [<Required>]
    [<Range(1, Int32.MaxValue)>]
    MaxResourcesCount : int
    
    RestrictionId : Nullable<int>
}

let ReadSubscriptionDTO id name description price monthDuration appliedResourceType maxResourcesCount restrictionId = {
    Id = id
    Name = name
    Description = description
    Price = price
    MonthDuration = monthDuration
    AppliedResourceType = appliedResourceType
    MaxResourcesCount = maxResourcesCount
    RestrictionId = restrictionId
}

[<CLIMutable>]
type ReadUserSubscriptionDTO = {
    [<Required>]
    Id : int
    
    [<Required>]
    SubscriptionId : int
    
    [<Required>]
    LeftResourcesCount : int
    
    [<Required>]
    DateFrom : string
    
    [<Required>]
    DateTo : string
}

let ReadUserSubscriptionDTO id subscriptionId leftResourcesCount dateFrom dateTo = {
    Id = id
    SubscriptionId = subscriptionId
    LeftResourcesCount = leftResourcesCount
    DateFrom = dateFrom
    DateTo = dateTo
}

[<CLIMutable>]
type CreateUserSubscriptionDTO = {
    [<Required>]
    UserId : int
    
    [<Required>]
    SubscriptionId : int
}

let CreateUserSubscriptionDTO userId subscriptionId = {
    UserId = userId
    SubscriptionId = subscriptionId
}

[<CLIMutable>]
type ReadRoleDTO = {
    [<Required>]
    Id : int
    
    [<Required>]
    Name : string
}

let ReadRoleDTO id name = {
    Id = id
    Name = name
}