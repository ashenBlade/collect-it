using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.Mocks;

public class InMemoryImageRepository : IImageRepository
{
    private readonly List<Image> _images;

    public InMemoryImageRepository()
    {
        _images = new()
                  {
                      new Image()
                      {
                          Resource = new Resource()
                                     {
                                         ResourceId = 1,
                                         ResourcePath = "/imagesFromDb/avaSig.jpg",
                                         UploadDate = DateTime.Today,
                                         ResourceOwner = new User()
                                                         {
                                                             UserName = "Игорь",
                                                             Id = 1
                                                         }
                                     },
                          ImageId = 1
                      }
                  };
    }
    public Task<int> AddAsync(Image item, Resource resource)
    {
        item.Resource = resource;
        item.ImageId = _images.Count;
        _images.Add(item);
        return Task.FromResult(item.ImageId);
    }

    public Task<Image?> FindByIdAsync(int id)
    {
        return Task.FromResult( _images.FirstOrDefault(i => i.ImageId == id) );
    }

    public Task UpdateAsync(Image item)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Image item, Resource resource)
    {
        _images.Remove(item);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<Image> GetAllByName(string name)
    {
        foreach (var image in _images)
        {
            yield return image;
        }
    }
}