namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class ResourceException : Exception
{
    public ResourceException(string? message = null)
        : base(message)
    {
    }
}