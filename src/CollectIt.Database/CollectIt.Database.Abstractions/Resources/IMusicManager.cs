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
    Task ChangeMusicNameAsync(int musicId, string name);
    Task ChangeMusicTagsAsync(int musicId, string[] tags);
}