using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class VideoRepository /*: IVideoRepository*/
{
    private readonly PostgresqlCollectItDbContext context;
    private readonly ResourceRepository resourceRepository;

    public VideoRepository(PostgresqlCollectItDbContext context)
    {
        this.context = context;
        resourceRepository = new ResourceRepository(context);
    }

    public async Task<int> AddAsync(Video item, Resource resource)
    {
        await context.Videos.AddAsync(item);
        await context.SaveChangesAsync();
        await resourceRepository.AddAsync(resource);
        return item.Id;
    }

    public async Task<Video> FindByIdAsync(int id)
    {
        return await context.Videos.Where(video => video.Id == id).SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Video item)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Video item, Resource resource)
    {
        context.Videos.Remove(item);
        await context.SaveChangesAsync();
        await resourceRepository.RemoveAsync(resource);
    }

    public IAsyncEnumerable<Video> GetAllByName(string name)
    {
        throw new NotImplementedException();
    }
}