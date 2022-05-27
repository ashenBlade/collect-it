using CollectIt.Database.Infrastructure.Resources.FileManagers;

namespace CollectIt.MVC.Infrastructure.Resources;

public class GenericPhysicalFileManager : IMusicFileManager, IVideoFileManager, IImageFileManager
{
    private readonly string _basePath;

    public GenericPhysicalFileManager(string basePath)
    {
        _basePath = basePath;
    }

    public Stream GetContent(string filename)
    {
        return File.Open(GetFullPath(filename), FileMode.Open);
    }

    public void Delete(string filename)
    {
        var file = new FileInfo(GetFullPath(filename));
        file.Delete();
    }

    public async Task<FileInfo> CreateAsync(string filename, Stream content)
    {
        var fullPath = GetFullPath(filename);
        await using var file = File.Open(fullPath, FileMode.CreateNew);
        await content.CopyToAsync(file);
        return new FileInfo(fullPath);
    }

    private string GetFullPath(string filename) => Path.Combine(_basePath, filename);
}