namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class SubscriptionException : AccountException
{
    public int SubscriptionId { get; set; }

    public SubscriptionException(int subscriptionId, string message = "")
        : base(message)
    {
        SubscriptionId = subscriptionId;
    }
}