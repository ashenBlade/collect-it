using System.IO;
using System.Threading.Tasks;
using CollectIt.Database.Infrastructure.Resources.FileManagers;

namespace CollectIt.API.Tests.Integration.Mocks;

public class StubVideoFileManager : IVideoFileManager
{
    public Stream GetContent(string filename)
    {
        return Stream.Null;
    }

    public void Delete(string filename)
    { }

    public async Task<FileInfo> CreateAsync(string filename, Stream content)
    {
        return new FileInfo(filename);
    }
}