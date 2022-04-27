using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Http;

namespace CollectIt.Database.Abstractions.Resources;

public interface IResourceManager<TItem>
{
    Task<int> AddAsync(TItem item);
    Task<TItem?> FindByIdAsync(int id);
    Task Create(int ownerId, string name, string[] tags, Stream uploadedFile, string extension, string address);
    string? GetExtension(string fileName);
    Task RemoveAsync(TItem item);
    IAsyncEnumerable<TItem> GetAllByQuery(string query, int pageNumber = 1, int pageSize = 15);
    
    Task<List<TItem>> GetAllPaged(int pageNumber, int pageSize);
    
    IAsyncEnumerable<TItem> GetAllByName(string name);
    IAsyncEnumerable<TItem> GetAllByTag(string tag);
}