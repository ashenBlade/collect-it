namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserNotFoundException : UserException
{
    public UserNotFoundException(int userId, string message = "") 
        : base(userId, message) 
    { }
}