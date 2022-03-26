using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Resources.Repositories;
using CollectIt.MVC.View.Models;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private readonly IImageRepository _imageRepository;

    private readonly IResourceRepository _resourceRepository;

    public ImagesController(IImageRepository imageRepository, 
                            IResourceRepository resourceRepository)
    {
        _imageRepository = imageRepository;
        _resourceRepository = resourceRepository;
        // _imageRepository = new ImageRepository(context);
        // _resourceRepository = new ResourceRepository(context);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetImagesByName(string query)
    {
        var images = new List<Image>();
        await foreach (var image in _imageRepository.GetAllByName(query))
        {
            images.Add(image);
        }

        return View("ImagesPage", new ImageCardsViewModel() {Images = images});
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = _imageRepository.FindByIdAsync(id).Result;
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