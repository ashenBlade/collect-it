namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class VideoNotFoundException : ResourceNotFoundException
{
    public VideoNotFoundException(int videoId, string message)
    :base(videoId, message)
    { }
}