using CollectIt.Database.Abstractions;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CollectIt.Database.Infrastructure.Resources.Managers;

public class PostgresqlVideoManager : IVideoManager
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly IVideoFileManager _fileManager;

    public PostgresqlVideoManager(PostgresqlCollectItDbContext context,
                                  IVideoFileManager fileManager)
    {
        _context = context;
        _fileManager = fileManager;
    }

    private static HashSet<string> Extensions { get; } = new()
                                                         {
                                                             "mp4",
                                                             "mov",
                                                             "wmv",
                                                             "avi",
                                                             "webm",
                                                             "avi",
                                                             "mkv"
                                                         };

    public async Task<Video> CreateAsync(string name,
                                         int ownerId,
                                         string[] tags,
                                         Stream content,
                                         string extension,
                                         int duration)
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidResourceCreationValuesException("Name is not provided");
        }

        if (tags is null)
        {
            throw new InvalidResourceCreationValuesException("Tags not provided");
        }

        if (content is null)
        {
            throw new InvalidResourceCreationValuesException("Content of video is not provided");
        }

        if (extension is null || string.IsNullOrWhiteSpace(extension))
        {
            throw new InvalidResourceCreationValuesException("Extension is not provided");
        }

        extension = extension.ToLower()
                             .Trim();

        if (!Extensions.Contains(extension))
        {
            throw new InvalidResourceCreationValuesException($"Extension '{extension}' is not supported");
        }

        if (duration < 1)
        {
            throw new InvalidResourceCreationValuesException($"Duration must be positive");
        }

        var filename = $"{Guid.NewGuid()}.{extension}";
        var video = new Video()
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
            var entity = await _context.Videos.AddAsync(video);
            video = entity.Entity;
            await _context.SaveChangesAsync();
            await _fileManager.CreateAsync(filename, content);
            return video;
        }
        catch (IOException)
        {
            _context.Videos.Remove(video);
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
                                                     new UserNotFoundException(video.OwnerId),
                                                 _ => p
                                             },
                      _ => db
                  };
        }
    }

    public Task<Video?> FindByIdAsync(int id)
    {
        return _context.Videos
                       .Include(v => v.Owner)
                       .SingleOrDefaultAsync(v => v.Id == id);
    }

    public async Task RemoveByIdAsync(int videoId)
    {
        var file = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId);
        if (file is null)
        {
            throw new VideoNotFoundException(videoId, "Video with provided id not found");
        }

        var filename = file.FileName;
        _fileManager.Delete(filename);
        _context.Videos.Remove(file);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Video>> GetPagedAsync(int pageNumber, int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        }

        return new PagedResult<Video>()
               {
                   Result = await _context.Videos
                                          .OrderBy(v => v.Id)
                                          .Skip(( pageNumber - 1 ) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(),
                   TotalCount = await _context.Videos.CountAsync()
               };
    }

    public async Task<PagedResult<Video>> QueryAsync(string query, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        var q = _context.Videos
                        .Where(v => v.TagsSearchVector.Matches(EF.Functions
                                                                 .WebSearchToTsQuery("russian", query)))
                        .OrderByDescending(v =>
                                               v.TagsSearchVector.Rank(EF.Functions
                                                                         .WebSearchToTsQuery("russian", query)));
        return new PagedResult<Video>()
               {
                   Result = await q
                                 .Include(v => v.Owner)
                                 .Skip(( pageNumber - 1 ) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync(),
                   TotalCount = await q.CountAsync()
               };
    }

    public async Task<PagedResult<Video>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        }

        return new PagedResult<Video>()
               {
                   Result = await _context.Videos
                                          .OrderBy(m => m.Id)
                                          .Include(m => m.Owner)
                                          .Skip(( pageNumber - 1 ) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(),
                   TotalCount = await _context.Videos.CountAsync()
               };
    }

    public async Task ChangeNameAsync(int videoId, string? name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId);
        if (video is null)
        {
            throw new VideoNotFoundException(videoId, "Video with provided id not found");
        }

        video.Name = name;
        _context.Videos.Update(video);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeTagsAsync(int videoId, string[]? tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId);
        if (video is null)
        {
            throw new VideoNotFoundException(videoId, "Video with provided id not found");
        }

        video.Tags = tags;
        _context.Videos.Update(video);
        await _context.SaveChangesAsync();
    }

    public async Task<Stream> GetContentAsync(int videoId)
    {
        var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId);
        if (video is null)
        {
            throw new VideoNotFoundException(videoId, "Video with specified id not found");
        }

        return _fileManager.GetContent(video.FileName);
    }

    public Task<bool> IsAcquiredBy(int videoId, int userId)
    {
        return _context.AcquiredUserResources
                       .AnyAsync(aur => aur.ResourceId == videoId && aur.UserId == userId);
    }
}