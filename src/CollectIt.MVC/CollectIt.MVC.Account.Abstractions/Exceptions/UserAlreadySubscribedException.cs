namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class UserAlreadySubscribedException : UserSubscriptionException
{
    public UserAlreadySubscribedException(int userId, int subscriptionId) 
        : base(userId, subscriptionId)
    { }
    
    public UserAlreadySubscribedException(int userId, int subscriptionId, string message) 
        : base(userId, subscriptionId, message)
    { }
}