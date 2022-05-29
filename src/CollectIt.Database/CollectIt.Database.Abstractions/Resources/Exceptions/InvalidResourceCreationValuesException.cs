namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class InvalidResourceCreationValuesException : ResourceException
{
    public InvalidResourceCreationValuesException(string? message = null)
        : base(message)
    {
    }
}