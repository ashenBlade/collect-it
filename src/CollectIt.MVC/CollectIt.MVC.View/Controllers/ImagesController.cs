using System.Collections;
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
    private readonly IImageManager _imageManager;
    private readonly ICommentManager _commentManager;
    private IWebHostEnvironment appEnvironment;

    public ImagesController(IImageManager imageManager,ICommentManager commentManager, IWebHostEnvironment appEnvironment)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
        this.appEnvironment = appEnvironment;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")] [Required] string query)
    {
        var images = new List<Image>();
        await foreach (var image in _imageManager.GetAllByQuery(query))
        {
            images.Add(image);
        }

        return View("Images", new ImageCardsViewModel() { Images = images });
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }
        var comments = await _commentManager.GetResourcesComments(source.Id);

        var commentViewModels = new List<CommentViewModel>();

        foreach (var comment in comments)
        {
            commentViewModels.Add(new CommentViewModel()
            {
                Content = comment.Content,
                Owner = comment.Owner,
                UploadDate = comment.UploadDate
            });
        }
        
        var model = new ImageViewModel()
        {
            Owner = source.Owner,
            UploadDate = source.UploadDate,
            Path = source.Address,
            Tags = source.Tags,
            Comments = commentViewModels
        };
        return View(model);
    }

    [Route("postView")]
    public IActionResult GetPostView()
    {
        return View("ImagePostPage");
    }

    [HttpPost]
    [Route("post")]
    public async Task<IActionResult> PostImage(string tags, string name, IFormFile uploadedFile)
    {
        var address = appEnvironment.WebRootPath + "/imagesFromDb/";
        await _imageManager.Create(address, uploadedFile.FileName, name, tags, uploadedFile);
        return View("ImagePostPage");
    }
}