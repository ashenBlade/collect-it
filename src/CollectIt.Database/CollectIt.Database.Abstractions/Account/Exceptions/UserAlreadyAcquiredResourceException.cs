using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserAlreadyAcquiredResourceException : UserException
{
    public int ResourceId { get; }

    public UserAlreadyAcquiredResourceException(int resourceId, int userId, string message = "") 
        : base(userId, message)
    {
        ResourceId = resourceId;
    }
}