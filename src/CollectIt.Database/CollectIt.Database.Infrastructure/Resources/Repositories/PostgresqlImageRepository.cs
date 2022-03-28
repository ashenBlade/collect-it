using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class PostgresqlImageRepository : IImageRepository
{    
    private readonly PostgresqlCollectItDbContext _context;

    public PostgresqlImageRepository(PostgresqlCollectItDbContext context)
    {
        _context = context;
    }
    

    public async Task<int> AddAsync(Image item)
    {
        await _context.Images.AddAsync(item);
        await _context.SaveChangesAsync();
        return item.Id;
    }

    public async Task<Image?> FindByIdAsync(int id)
    {
        return await _context.Images
            .Where(img => img.Id == id)
                             .Include(img => img.Owner)
            .SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Image item)
    {
        _context.Images.Update(item);
        return _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Image item)
    {
        _context.Images.Remove(item);
        await _context.SaveChangesAsync();
    }

    public IAsyncEnumerable<Image> GetAllByName(string name)
    {
        return _context.Images
                       .Where(img => img.NameSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", name)))
                       .Include(img => img.Owner)
                       .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Image> GetAllByTag(string tag)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Image> GetAllByQuery(string query)
    {
        return _context.Images
                       .Where(img => img.NameSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", query)))
                       .Include(img => img.Owner)
                       .AsAsyncEnumerable();
    }
}