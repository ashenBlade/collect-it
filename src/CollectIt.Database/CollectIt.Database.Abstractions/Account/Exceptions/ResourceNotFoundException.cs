namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class ResourceNotFoundException : ApplicationException
{
    public int ResourceId { get; set; }

    public ResourceNotFoundException(int resourceId = 0, string message = "")
        : base(message)
    {
        
    }
}