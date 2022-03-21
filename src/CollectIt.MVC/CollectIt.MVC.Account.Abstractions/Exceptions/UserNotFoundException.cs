namespace CollectIt.MVC.Account.Abstractions.Exceptions;

public class UserNotFoundException : UserException
{
    public UserNotFoundException(int userId, string message = "") 
        : base(userId, message) 
    { }
}