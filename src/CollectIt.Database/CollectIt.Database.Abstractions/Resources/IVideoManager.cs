using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IVideoManager
{
    Task<Video> CreateAsync(string name,
                            int ownerId,
                            string[] tags,
                            Stream content,
                            string extension,
                            int duration);
    Task<Video?> FindByIdAsync(int id);
    Task RemoveByIdAsync(int videoId);
    Task<PagedResult<Video>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<Video>> QueryAsync(string query, int pageNumber, int pageSize);
    Task<PagedResult<Video>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task ChangeNameAsync(int videoId, string name);
    Task ChangeTagsAsync(int videoId, string[] tags);
    Task<Stream> GetContentAsync(int videoId);
    Task<bool> IsAcquiredBy(int videoId, int userId);
}