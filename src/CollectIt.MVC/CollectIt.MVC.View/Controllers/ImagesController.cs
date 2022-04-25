﻿using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private readonly IImageManager _imageManager;
    private readonly ICommentManager _commentManager;
    private readonly UserManager _userManager;

    private readonly string address;

    private static readonly int MaxPageSize = 5;

    public ImagesController(IImageManager imageManager, IWebHostEnvironment appEnvironment, UserManager userManager,
                            ICommentManager commentManager)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
        _userManager = userManager;
        address = Path.Combine(Directory.GetCurrentDirectory(), "content", "images");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")]
                                                     [Required] 
                                                     string query, 
                                                     [FromQuery(Name = "p")]
                                                     [Range(1, int.MaxValue)]
                                                     int pageNumber = 1)
    {
        var images = new List<Image>();
        await foreach (var image in _imageManager.GetAllByQuery(query, pageNumber, MaxPageSize))
        {
            images.Add(image);
        }

        return View("Images", new ImageCardsViewModel() 
                              { 
                                  Images = images.Select(i => new ImageViewModel(){
                                                                                      Address = $"/imagesFromDb/{i.FileName}",
                                                                                      Name = i.Name,
                                                                                      ImageId = i.Id,
                                                                                      Comments = Array.Empty<CommentViewModel>(),
                                                                                      Tags = i.Tags,
                                                                                      OwnerName = i.Owner.UserName,
                                                                                      UploadDate = i.UploadDate,
                                                                                      IsAcquired = false
                                                                                  }).ToList(), 
                                  PageNumber = pageNumber,
                                  MaxImagesCount = MaxPageSize,
                                  Query = query
                              });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var comments = await _commentManager.GetResourcesComments(source.Id);

        var model = new ImageViewModel()
                    {
                        ImageId = id,
                        Comments = comments.Select(c => new CommentViewModel()
                                                        { Author = c.Owner.UserName, PostTime = c.UploadDate, Comment = c.Content }),
                        Name = source.Name,
                        OwnerName = source.Owner.UserName,
                        UploadDate = source.UploadDate,
                        Address = $"/imagesFromDb/{source.FileName}",
                        Tags = source.Tags,
                        IsAcquired = await _imageManager.IsAcquiredBy(source.OwnerId, id)
                    };
        return View(model);
    }

    [HttpGet("postView")]
    public IActionResult GetPostView()
    {
        return View("ImagePostPage");
    }

    [HttpPost("post")]
    public async Task<IActionResult> PostImage(string tags, string name, IFormFile uploadedFile)
    {
        var ext = _imageManager.GetExtension(uploadedFile.FileName);
        if (ext is null)
            return View("Error");
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Login", "Account");
        await using var stream = uploadedFile.OpenReadStream();
        await _imageManager.Create(user.Id, address, name, tags, stream, ext);
        return View("ImagePostPage");
    }
    

    [Route("download/{id:int}")]
    public async Task<IActionResult> DownloadImage(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var file = new FileInfo(Path.Combine(address, source.FileName));

        return file.Exists
                   ? PhysicalFile(file.FullName, $"image/jpg")
                   : BadRequest(new {Message = "File not found"});
    }


    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> LeaveComment([FromForm] LeaveCommentVewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var imageId = model.ImageId;
        var comment = await _commentManager.CreateComment(imageId, user.Id, model.Content);
        if (comment is null)
            return BadRequest($"This feature is not implemented yet\nImage Id: {imageId}\nComment: {model.Content}");
        return RedirectToAction("Image", new { id = model.ImageId });
    }
}