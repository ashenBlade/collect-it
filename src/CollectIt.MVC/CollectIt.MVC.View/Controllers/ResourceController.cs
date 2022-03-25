using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Resources.Repositories;
using CollectIt.MVC.View.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("resource")]
public class ResourceController : Controller
{
    private ImageRepository imageRepository;
    private ResourceRepository resourceRepository;

    public ResourceController(PostgresqlCollectItDbContext context)
    {
        imageRepository = new ImageRepository(context);
        resourceRepository = new ResourceRepository(context);
    }

    [HttpGet]
    [Route("image")]
    public async Task<IActionResult> Image(int id)
    {
        var source = imageRepository.FindByIdAsync(id).Result;
        if (source == null)
            return View("Error");
        var imgModel = new ImageViewModel()
        {
            Owner = source.Resource.ResourceOwner,
            UploadDate = source.Resource.UploadDate,
            Path = source.Resource.ResourcePath
        };
        return View(imgModel);
    }
}