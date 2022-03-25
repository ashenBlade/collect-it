using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class ImageRepository : IImageRepository
{    
    private readonly PostgresqlCollectItDbContext context;
    private readonly ResourceRepository resourceRepository;

    public ImageRepository(PostgresqlCollectItDbContext context)
    {
        this.context = context;
        resourceRepository = new ResourceRepository(context);
    }
    

    public async Task<int> AddAsync(Image item, Resource resource)
    {
        await context.Images.AddAsync(item);
        await context.SaveChangesAsync();
        await resourceRepository.AddAsync(resource);
        return item.ImageId;
    }

    public async Task<Image?> FindByIdAsync(int id)
    {
        return await context.Images
            .Include(img => img.Resource)
            .ThenInclude(res => res.ResourceOwner)
            .Where(img => img.ImageId == id).SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Image item)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Image item, Resource resource)
    {
        context.Images.Remove(item);
        await resourceRepository.RemoveAsync(resource);
        await context.SaveChangesAsync();
    }
}