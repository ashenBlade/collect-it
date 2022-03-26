namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class SubscriptionNotFoundException : SubscriptionException
{
    public SubscriptionNotFoundException( int subscriptionId) : base(subscriptionId) 
    { }
    public SubscriptionNotFoundException(int subscriptionId, string message) : base(subscriptionId, message) 
    { }
}