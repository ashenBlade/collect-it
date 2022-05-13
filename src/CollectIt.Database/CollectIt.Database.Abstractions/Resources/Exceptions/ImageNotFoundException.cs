namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class ImageNotFoundException : ResourceNotFoundException
{
    
    public ImageNotFoundException(int imageId, string message)
        :base(imageId, message)
    { }
}