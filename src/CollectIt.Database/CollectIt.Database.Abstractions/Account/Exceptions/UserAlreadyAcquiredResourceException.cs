namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserAlreadyAcquiredResourceException : UserException
{
    public UserAlreadyAcquiredResourceException(int resourceId, int userId, string message = "")
        : base(userId, message)
    {
        ResourceId = resourceId;
    }

    public int ResourceId { get; }
}