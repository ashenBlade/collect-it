using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IImageManager 
{
    Task<Image?> FindByIdAsync(int id);
    Task<Image> CreateAsync(string name, 
        int ownerId, 
        string[] tags, 
        Stream content, 
        string extension);
    Task RemoveByIdAsync(int id);
    Task<PagedResult<Image>> QueryAsync(string query, int pageNumber, int pageSize);
    Task<PagedResult<Image>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task ChangeNameAsync(int id, string name);
    Task ChangeTagsAsync(int id, string[] tags);
    Task<bool> IsAcquiredBy(int id, int userId);
    Task<Stream> GetContentAsync(int id);
}