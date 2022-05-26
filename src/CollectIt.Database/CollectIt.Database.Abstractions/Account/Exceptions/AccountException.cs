namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class AccountException : Exception
{
    public AccountException(string message = "")
        : base(message)
    {
    }
}