using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IImageManager : IResourceManager<Image>
{
    public Task<bool> IsAcquiredBy(int userId, int imageId);
    public Task<PagedResult<Image>> QueryAsync(string query, int pageSize, int pageNumber);
}