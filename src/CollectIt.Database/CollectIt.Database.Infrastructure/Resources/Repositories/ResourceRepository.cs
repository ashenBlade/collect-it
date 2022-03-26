using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly PostgresqlCollectItDbContext context;

    public ResourceRepository(PostgresqlCollectItDbContext context)
    {
        this.context = context;
    }

    public async Task<int> AddAsync(Resource item)
    {
        await context.Resources.AddAsync(item);
        await context.SaveChangesAsync();
        return item.Id;
    }

    public async Task<Resource> FindByIdAsync(int id)
    {
        return await context.Resources.Where(resource => resource.Id == id).SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Resource item)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Resource item)
    {
        context.Resources.Remove(item);
        await context.SaveChangesAsync();
    }
}