namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserSubscriptionException : AccountException
{
    public UserSubscriptionException(int userId, int subscriptionId)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
    }

    public int UserId { get; }
    public int SubscriptionId { get; }
}