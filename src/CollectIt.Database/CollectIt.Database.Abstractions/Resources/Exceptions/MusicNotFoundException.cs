namespace CollectIt.Database.Abstractions.Resources.Exceptions;

public class MusicNotFoundException : ResourceNotFoundException
{
    
    public MusicNotFoundException(int musicId, string message)
        :base(musicId, message)
    { }
}