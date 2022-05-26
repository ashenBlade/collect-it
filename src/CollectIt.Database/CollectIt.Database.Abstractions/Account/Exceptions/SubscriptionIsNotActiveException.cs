namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class SubscriptionIsNotActiveException : SubscriptionException
{
    public SubscriptionIsNotActiveException(int subscriptionId)
        : base(subscriptionId, $"Subscription with id = {subscriptionId} is not active")
    {
    }
}