using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

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
    

    public async Task<int> AddAsync(Image item)
    {
        await context.Images.AddAsync(item);
        await context.SaveChangesAsync();
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

    public async Task RemoveAsync(Image item)
    {
        context.Images.Remove(item);
        await context.SaveChangesAsync();
    }
    
    public async void DownloadImage(IFormFile uploadedFile, string path)
    {
        await using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await uploadedFile.CopyToAsync(fileStream);
        }
    }
}