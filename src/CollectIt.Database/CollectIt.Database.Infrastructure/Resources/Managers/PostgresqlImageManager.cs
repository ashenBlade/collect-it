﻿using CollectIt.Database.Abstractions.Resources;
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

    public async Task Create(int ownerId, string name, string[] tags, Stream uploadedFile, string extension, string address)
    {
        var fileName = $"{Guid.NewGuid()}.{extension}";
        await using (var fileStream = new FileStream(Path.Combine(address, fileName), FileMode.Create))
        {
            await uploadedFile.CopyToAsync(fileStream);
        }

        var image = new Image()
        {
            Tags = tags,
            Name = name,
            FileName = fileName,
            UploadDate = DateTime.UtcNow,
            Extension = extension,
            OwnerId = ownerId
        };
        await AddAsync(image);
    }
    
    public string? GetExtension(string fileName)
    {
        var ext = fileName.Split(".").Last();
        if (ext != "jpg" && ext != "png")
            return null;
        return fileName.Split(".").Last() == "jpg" ? "jpeg" : "png";
    }
    
    private string RenamePostedImage(string name)
    {
        return name.Replace(" ", "-").ToLower() + "-" + (_context.Images.Count() + 1); 
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