using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Http;

namespace CollectIt.Database.Abstractions.Resources;

public interface IResourceManager<TItem>
{
    Task<int> AddAsync(TItem item);
    Task<TItem?> FindByIdAsync(int id);
    Task<TItem> Create(int ownerId, string address, string name, string tags, Stream uploadedFile, string extension);
    string? GetExtension(string fileName);
    Task RemoveAsync(int id);
    IAsyncEnumerable<TItem> GetAllByQuery(string query, int pageNumber = 1, int pageSize = 15);
    
    Task<List<TItem>> GetAllPaged(int pageNumber, int pageSize);
    
    IAsyncEnumerable<TItem> GetAllByName(string name);
    IAsyncEnumerable<TItem> GetAllByTag(string tag);
    Task ChangeNameAsync(int id, string name);
    Task ChangeTagsAsync(int id, string[] tags);
    
}