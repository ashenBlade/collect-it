namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class SubscriptionException : AccountException
{
    public int SubscriptionId { get; set; }

    public SubscriptionException(int subscriptionId, string message = "")
        : base(message)
    {
        SubscriptionId = subscriptionId;
    }
}