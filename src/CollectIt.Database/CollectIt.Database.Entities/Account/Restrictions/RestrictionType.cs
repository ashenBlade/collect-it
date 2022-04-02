namespace CollectIt.Database.Entities.Account.Restrictions;

public enum RestrictionType
{
    DenyAll = 0,
    AllowAll = 1,
    DaysAfter = 2,
    DaysTo = 3,
    Size = 4,
    Author = 5,
    Tags = 6
}