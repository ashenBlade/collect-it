namespace CollectIt.Database.Abstractions.Account.Exceptions;

public class VideoNotFoundException : ResourceNotFoundException
{
    public VideoNotFoundException(int videoId, string message)
    :base(videoId, message)
    { }
}