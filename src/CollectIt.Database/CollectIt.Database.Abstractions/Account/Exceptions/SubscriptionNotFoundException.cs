namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class SubscriptionNotFoundException : SubscriptionException
{
    public SubscriptionNotFoundException( int subscriptionId) : base(subscriptionId) 
    { }
    public SubscriptionNotFoundException(int subscriptionId, string message) : base(subscriptionId, message) 
    { }
}