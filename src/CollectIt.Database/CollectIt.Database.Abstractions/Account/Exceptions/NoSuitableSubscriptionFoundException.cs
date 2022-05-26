namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class NoSuitableSubscriptionFoundException : AcquisitionException
{
    public NoSuitableSubscriptionFoundException(string message)
        : base(message)
    {
    }
}