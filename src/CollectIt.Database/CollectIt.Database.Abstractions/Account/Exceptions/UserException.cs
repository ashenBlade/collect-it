namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class UserException : AccountException
{
    public int UserId { get; set; }

    public UserException(int userId, string message = "")
        : base(message)
    {
        UserId = userId;
    }
}