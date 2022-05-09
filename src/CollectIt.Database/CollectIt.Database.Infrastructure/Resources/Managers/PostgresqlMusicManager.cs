using CollectIt.Database.Abstractions;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Managers;

public class PostgresqlMusicManager : IMusicManager
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly IMusicFileManager _fileManager;

    public PostgresqlMusicManager(PostgresqlCollectItDbContext context, IMusicFileManager fileManager)
    {
        _context = context;
        _fileManager = fileManager;
    }

    public Task<Music?> FindByIdAsync(int id)
    {
        return _context.Musics
                       .Include(m => m.Owner)
                       .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Music> CreateAsync(string name,
                                         int ownerId,
                                         string[] tags,
                                         Stream content,
                                         string extension,
                                         int duration)
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Music name can not be null or empty");
        }

        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (extension is null || string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentOutOfRangeException(nameof(extension), "Music extension can not be null or empty");
        }

        if (duration < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Music duration must be positive");
        }

        var filename = $"{new Guid()}.{extension}";
        var music = new Music()
                    {
                        Name = name,
                        Extension = extension,
                        Duration = duration,
                        OwnerId = ownerId,
                        Tags = tags,
                        FileName = filename,
                        UploadDate = DateTime.UtcNow,
                    };
        try
        {
            var entity = await _context.Musics.AddAsync(music);
            music = entity.Entity;
            await _context.SaveChangesAsync();
            var file = await _fileManager.CreateAsync(filename, content);
            return music;
        }
        catch (IOException)
        {
            _context.Musics.Remove(music);
            await _context.SaveChangesAsync();
            throw;
        }
    }

    public Task RemoveByIdAsync(int musicId)
    {
        _context.Musics.Remove(new Music() {Id = musicId});
        return _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Music>> QueryAsync(string query, int pageNumber, int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        }

        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var q = _context.Musics
                        .Where(m => m.TagsSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", query)))
                        .OrderByDescending(m => m.TagsSearchVector.Rank(EF.Functions.WebSearchToTsQuery("russian",
                                                                                                        query)));
        var x = await q.Select(m => new
                                    {
                                        All = q.Include(m => m.Owner)
                                               .Skip(( pageNumber - 1 ) * pageNumber)
                                               .ToList(),
                                        Count = q.Count()
                                    })
                       .FirstOrDefaultAsync();
        return new PagedResult<Music>() {Result = x.All, TotalCount = x.Count};
    }

    public async Task<PagedResult<Music>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        }

        return new PagedResult<Music>()
               {
                   Result = await _context.Musics
                                          .OrderBy(m => m.Id)
                                          .Include(m => m.Owner)
                                          .Skip(( pageNumber - 1 ) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(),
                   TotalCount = await _context.Musics.CountAsync()
               };
    }

    public async Task ChangeMusicNameAsync(int musicId, string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var music = await _context.Musics.SingleOrDefaultAsync(m => m.Id == musicId);
        if (music is null)
        {
            throw new ResourceNotFoundException(musicId, "Music with specified id not found");
        }

        music.Name = name;
        await _context.SaveChangesAsync();
    }

    public async Task ChangeMusicTagsAsync(int musicId, string[] tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var music = await _context.Musics.SingleOrDefaultAsync(m => m.Id == musicId);
        if (music is null)
        {
            throw new ResourceNotFoundException(musicId, "Music with specified id not found");
        }

        music.Tags = tags;
        await _context.SaveChangesAsync();
    }

    public Task<bool> IsAcquiredBy(int musicId, int userId)
    {
        return _context.AcquiredUserResources
                       .AnyAsync(m => m.ResourceId == musicId && m.UserId == userId);
    }

    public async Task<Stream> GetContentAsync(int musicId)
    {
        var file = await _context.Musics.SingleOrDefaultAsync(m => m.Id == musicId);
        if (file is null)
        {
            throw new ResourceNotFoundException(musicId, "Music with provided id not found");
        }

        var filename = file.Name;
        return _fileManager.GetContent(filename);
    }
}