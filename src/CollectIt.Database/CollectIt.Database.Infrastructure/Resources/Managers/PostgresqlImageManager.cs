using CollectIt.Database.Abstractions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Managers;

public class PostgresqlImageManager : IImageManager
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly IImageFileManager _fileManager;

    public PostgresqlImageManager(PostgresqlCollectItDbContext context, IImageFileManager fileManager)
    {
        _context = context;
        _fileManager = fileManager;
    }

    public async Task<Image?> FindByIdAsync(int id)
    {
        return await _context.Images
            .Where(img => img.Id == id)
            .Include(img => img.Owner)
            .SingleOrDefaultAsync();
    }

    public async Task<Image> CreateAsync(string name, int ownerId, string[] tags, Stream content, string extension)
    {
        if (name is null || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Image name can not be null or empty");
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
            throw new ArgumentOutOfRangeException(nameof(extension), "Image extension can not be null or empty");
        }

        var filename = $"{new Guid()}.{extension}";
        var image = new Image()
        {
            Name = name,
            Extension = extension,
            OwnerId = ownerId,
            Tags = tags,
            FileName = filename,
            UploadDate = DateTime.UtcNow,
        };
        try
        {
            var entity = await _context.Images.AddAsync(image);
            image = entity.Entity;
            await _context.SaveChangesAsync();
            var file = await _fileManager.CreateAsync(filename, content);
            return image;
        }
        catch (IOException)
        {
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            throw;
        }
    }

    public Task RemoveByIdAsync(int id)
    {
        _context.Images.Remove(new Image() {Id = id});
        return _context.SaveChangesAsync();
    }

    public async Task ChangeNameAsync(int id, string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var image = await _context.Images.SingleOrDefaultAsync(img => img.Id == id);
        if (image is null)
        {
            throw new ImageNotFoundException(id, "Image with provided id not found");
        }

        image.Name = name;
        _context.Images.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeTagsAsync(int id, string[]? tags)
    {
        if (tags is null)
        {
            throw new ArgumentNullException(nameof(tags));
        }

        var image = await _context.Images.SingleOrDefaultAsync(img => img.Id == id);
        if (image is null)
        {
            throw new ImageNotFoundException(id, "Image with provided id not found");
        }

        image.Tags = tags;
        _context.Images.Update(image);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Image>> GetAllPaged(int pageNumber, int pageSize)
    {
        return await _context.Images
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
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

    public Task<bool> IsAcquiredBy(int userId, int imageId)
    {
        return _context.AcquiredUserResources
            .AnyAsync(aur => aur.ResourceId == imageId && aur.UserId == userId);
    }

    public async Task<Stream> GetContentAsync(int id)
    {
        var image = await _context.Images.SingleOrDefaultAsync(img => img.Id == id);
        if (image is null)
        {
            throw new ImageNotFoundException(id, "Image with specified id not found");
        }

        return _fileManager.GetContent(image.FileName);
    }

    public async Task<PagedResult<Image>> QueryAsync(string query, int pageSize, int pageNumber)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        }


        var q = _context.Images
            .Where(img => img.TagsSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", query)))
            .Include(img => img.Owner)
            .OrderByDescending(img =>
                img.TagsSearchVector.Rank(EF.Functions.WebSearchToTsQuery("russian",
                    query)));

        return new PagedResult<Image>()
        {
            Result = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            TotalCount = await q
                .CountAsync()
        };
    }

    public IAsyncEnumerable<Image> GetAllByQuery(string query, int pageNumber = 1, int pageSize = 15)
    {
        return _context.Images
            .Where(img => img.TagsSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", query)))
            .Include(img => img.Owner)
            .OrderByDescending(img =>
                img.TagsSearchVector.Rank(EF.Functions.WebSearchToTsQuery("russian",
                    query)))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsAsyncEnumerable();
    }

    public async Task<PagedResult<Image>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be positive");
        }

        return new PagedResult<Image>()
        {
            Result = await _context.Images
                .OrderBy(m => m.Id)
                .Include(m => m.Owner)
                .Skip(( pageNumber - 1 ) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            TotalCount = await _context.Images.CountAsync()
        };
    }
}