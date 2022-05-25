namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class ImageNotFoundException : ResourceNotFoundException
{
    public ImageNotFoundException(int imageId, string? message = null)
        : base(imageId, message ?? $"Image with id = {imageId} not found")
    {
    }
}