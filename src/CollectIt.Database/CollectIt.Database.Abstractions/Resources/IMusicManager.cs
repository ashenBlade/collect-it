using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IMusicManager
{
    Task<Music?> FindByIdAsync(int id);
    Task<Music> CreateAsync(string name, 
                            int ownerId, 
                            string[] tags, 
                            Stream content, 
                            string extension, 
                            int duration);
    Task RemoveByIdAsync(int musicId);
    Task<PagedResult<Music>> QueryAsync(string query, int pageNumber, int pageSize);
    Task<PagedResult<Music>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task ChangeNameAsync(int musicId, string name);
    Task ChangeTagsAsync(int musicId, string[] tags);
    Task<bool> IsAcquiredBy(int musicId, int userId);
    Task<Stream> GetContentAsync(int musicId);
}