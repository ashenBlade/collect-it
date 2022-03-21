namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class UserSubscriptionException : AccountException
{
    public int UserId { get; }
    public int SubscriptionId { get; }

    public UserSubscriptionException(int userId, int subscriptionId)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
    }
    
    public UserSubscriptionException(int userId, int subscriptionId, string message)
        : base(message)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
    }
}