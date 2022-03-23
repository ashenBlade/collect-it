namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class AccountException : Exception
{
    public AccountException(string message = "")
     : base(message)
    {
        
    }
}