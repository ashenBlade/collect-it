using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class MusicManager : IMusicManager
{
    private readonly PostgresqlCollectItDbContext context;

    public MusicManager(PostgresqlCollectItDbContext context)
    {
        this.context = context;
    }

    public async Task<int> AddAsync(Music item)
    {
        await context.Musics.AddAsync(item);
        await context.SaveChangesAsync();
        return item.Id;
    }

    public async Task<Music?> FindByIdAsync(int id)
    {
        return await context.Musics
            .Where(music => music.Id == id)
            .SingleOrDefaultAsync();
    }

    public Task Create(string address, string fileName, string name, string tags, IFormFile uploadedFile)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Music item)
    {
        context.Musics.Remove(item);
        await context.SaveChangesAsync();
    }

    public IAsyncEnumerable<Music> GetAllByQuery(string query)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Music> GetAllByName(string name)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Music> GetAllByTag(string tag)
    {
        throw new NotImplementedException();
    }
}