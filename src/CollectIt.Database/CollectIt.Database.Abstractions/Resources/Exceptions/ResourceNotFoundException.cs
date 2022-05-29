namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class ResourceNotFoundException : ResourceException
{
    public ResourceNotFoundException(int resourceId = 0, string message = "")
        : base(message)
    {
    }

    public int ResourceId { get; set; }
}