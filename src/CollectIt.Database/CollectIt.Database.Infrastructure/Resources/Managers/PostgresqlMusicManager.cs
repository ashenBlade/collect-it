using CollectIt.Database.Abstractions;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

    private static HashSet<string> SupportedExtensions { get; } =
        new() {"mp3", "aac", "wav", "flac"};

    public async Task<Music?> FindByIdAsync(int id)
    {
        return await _context.Musics
                             .Where(mus => mus.Id == id)
                             .Include(mus => mus.Owner)
                             .SingleOrDefaultAsync();
    }

    private static readonly HashSet<string> Extensions = new() {"mp3", "ogg", "wav"};

    public async Task<Music> CreateAsync(string name,
                                         int ownerId,
                                         string[] tags,
                                         Stream content,
                                         string extension,
                                         int duration)
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidResourceCreationValuesException("Music name can not be null or empty");
        }

        if (tags is null)
        {
            throw new InvalidResourceCreationValuesException("Tags were not provided");
        }

        if (content is null)
        {
            throw new InvalidResourceCreationValuesException("Content was not provided");
        }

        if (extension is null || string.IsNullOrWhiteSpace(extension))
        {
            throw new InvalidResourceCreationValuesException("Music extension can not be null or empty");
        }
        
        if (!Extensions.Contains(extension))
        {
            throw new InvalidResourceCreationValuesException($"Extension '{extension}' is not supported");
        }

        extension = extension.ToLower()
                             .Trim();
        if (!SupportedExtensions.Contains(extension))
        {
            throw new InvalidResourceCreationValuesException($"Music extension '{extension}' is not supported");
        }

        if (duration < 1)
        {
            throw new InvalidResourceCreationValuesException("Music duration must be positive");
        }

        var filename = $"{Guid.NewGuid()}.{extension}";
        var music = new Music()
                    {
                        Duration = duration,
                        Name = name,
                        Tags = tags,
                        FileName = filename,
                        OwnerId = ownerId,
                        Extension = extension,
                        UploadDate = DateTime.UtcNow,
                    };
        try
        {
            var entity = await _context.Musics.AddAsync(music);
            music = entity.Entity;
            await _context.SaveChangesAsync();
            await _fileManager.CreateAsync(filename, content);
            return music;
        }
        catch (IOException ioException)
        {
            _context.Musics.Remove(music);
            await _context.SaveChangesAsync();
            throw;
        }
        catch (DbUpdateException db)
        {
            throw db.InnerException switch
                  {
                      PostgresException p => p.ConstraintName switch
                                             {
                                                 "FK_Resources_AspNetUsers_OwnerId" =>
                                                     new UserNotFoundException(music.OwnerId),
                                                 _ => p
                                             },
                      _ => db
                  };
        }
    }

    public Task RemoveByIdAsync(int musicId)
    {
        _context.Musics.Remove(new Music() {Id = musicId});
        return _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Music>> QueryAsync(string query, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        var q = _context.Musics
                        .Where(mus => mus.TagsSearchVector.Matches(EF.Functions
                                                                     .WebSearchToTsQuery("russian", query)))
                        .OrderByDescending(mus =>
                                               mus.TagsSearchVector.Rank(EF.Functions
                                                                           .WebSearchToTsQuery("russian", query)));
        return new PagedResult<Music>()
               {
                   Result = await q
                                 .Include(v => v.Owner)
                                 .Skip(( pageNumber - 1 ) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync(),
                   TotalCount = await q.CountAsync()
               };
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

    public async Task ChangeNameAsync(int musicId, string name)
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

    public async Task ChangeTagsAsync(int musicId, string[] tags)
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

        var filename = file.FileName;
        return _fileManager.GetContent(filename);
    }
}