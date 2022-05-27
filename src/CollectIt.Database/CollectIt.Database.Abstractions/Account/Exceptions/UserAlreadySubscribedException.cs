namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserAlreadySubscribedException : UserSubscriptionException
{
    public UserAlreadySubscribedException(int userId, int subscriptionId)
        : base(userId, subscriptionId)
    {
    }
}