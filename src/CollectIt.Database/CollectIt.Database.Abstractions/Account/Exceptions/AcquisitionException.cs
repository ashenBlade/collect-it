namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class AcquisitionException : AccountException
{
    public AcquisitionException(string message)
        : base(message)
    {
    }
}