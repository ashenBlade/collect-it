using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class PostgresqlImageManager : IImageManager
{    
    private readonly PostgresqlCollectItDbContext _context;

    public PostgresqlImageManager(PostgresqlCollectItDbContext context)
    {
        _context = context;
    }

    public async Task Create(string address,string fileName, string name, string tags, IFormFile uploadedFile)
    {
        var tagsArray = tags.Split(" ");
        await using (var fileStream = new FileStream(address + fileName, FileMode.Create))
        {
            await uploadedFile.CopyToAsync(fileStream);
        }

        var image = new Image()
                    {
                        Address = address,
                        Tags = tagsArray,
                        Name = name,
                        FileName = fileName,
                        UploadDate = DateTime.UtcNow,
                        Extension = GetExtension(fileName),
                        //Заглушка дальше
                        Owner = await _context.Users.FirstOrDefaultAsync()
                    };
        await AddAsync(image);
    }

    private string GetExtension(string fileName)
    {
        return fileName.Split(".").Last();
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

    public async Task<List<Image>> GetAllPaged(int pageNumber, int pageSize)
    {
        return await _context.Images
                             .Skip(( pageNumber - 1 ) * pageSize)
                             .Take(pageSize).ToListAsync();
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
                       .AnyAsync(aus => aus.UserId == userId && aus.ResourceId == imageId);
    }

    public IAsyncEnumerable<Image> GetAllByQuery(string query, int pageNumber = 1, int pageSize = 15)
    {
        return _context.Images
                       .Where(img => img.TagsSearchVector.Matches(EF.Functions.WebSearchToTsQuery("russian", query)))
                       .Include(img => img.Owner)
                       .OrderByDescending(img => img.TagsSearchVector.Rank(EF.Functions.WebSearchToTsQuery("russian", query)))
                       .Skip((pageNumber - 1) * pageSize)
                       .Take(pageSize)
                       .AsAsyncEnumerable();
    }
}