using System.ComponentModel.DataAnnotations;
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


    public ImagesController(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")][Required]string query)
    {
        var images = new List<Image>();
        await foreach (var image in _imageRepository.GetAllByQuery(query))
        {
            images.Add(image);
        }

        return View("Images", new ImageCardsViewModel() {Images = images});
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = await _imageRepository.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }
        var model = new ImageViewModel()
                       {
                           Owner = source.Owner,
                           UploadDate = source.UploadDate,
                           Path = source.Address,
                       };
        return View(model);
    }
}