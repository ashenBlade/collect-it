using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Http;

namespace CollectIt.Database.Abstractions.Resources;

public interface IResourceManager<TItem>
{
    Task<int> AddAsync(TItem item);
    Task<TItem?> FindByIdAsync(int id);
    Task Create(int ownerId, string address, string fileName, string name, string tags, IFormFile uploadedFile);
    Task RemoveAsync(TItem item);
    IAsyncEnumerable<TItem> GetAllByQuery(string query);
    
    Task<List<TItem>> GetAllPaged(int pageNumber, int pageSize);
    
    IAsyncEnumerable<TItem> GetAllByName(string name);
    IAsyncEnumerable<TItem> GetAllByTag(string tag);
}