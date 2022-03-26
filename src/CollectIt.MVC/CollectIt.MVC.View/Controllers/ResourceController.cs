using CollectIt.Database.Entities.Resources;
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
    private IWebHostEnvironment appEnvironment;

    public ResourceController(PostgresqlCollectItDbContext context, IWebHostEnvironment appEnvironment)
    {
        imageRepository = new ImageRepository(context);
        resourceRepository = new ResourceRepository(context);
        this.appEnvironment = appEnvironment;
    }

    [HttpGet]
    [Route("images")]
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

    [HttpPost]
    [Route("addImage")]
    public async Task<IActionResult> AddImage(IFormFile uploadedFile, ImageViewModel imageViewModel)
    {
        var path = appEnvironment.WebRootPath +  "/imagesFromDb/" + uploadedFile.FileName;
        var resource = new Resource()
        {
            ResourceOwner = imageViewModel.Owner,
            ResourcePath = path,
            UploadDate = DateTime.Now
        };
        int resId = await resourceRepository.AddAsync(resource);
        imageRepository.DownloadImage(uploadedFile, path);
        var image = new Image()
        {
            Resource = resource
        };
        await imageRepository.AddAsync(image);
        return View("Error");
    }
}