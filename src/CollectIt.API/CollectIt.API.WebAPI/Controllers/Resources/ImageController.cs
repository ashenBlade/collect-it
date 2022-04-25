using System.ComponentModel.DataAnnotations;
using System.Text;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.Controllers.Resources;

[ApiController]
[Route("api/images")]
public class ImageController : Controller
{
    private IImageManager _imageManager;
    private UserManager _userManager;
    private string _address;

    public ImageController(IImageManager imageManager, UserManager userManager)
    {
        _imageManager = imageManager;
        _userManager = userManager;
        _address = Path.Combine(Directory.GetCurrentDirectory(), "content", "images");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindImageById(int id)
    {
        var image = await _imageManager.FindByIdAsync(id);
        if (image is null)
            return NotFound();
        return Ok(ResourcesMappers.ToReadImageDTO(image));
    }

   /* public async Task<IActionResult> FindPhysicalImageById(int id)
    {
        var image = await _imageManager.FindByIdAsync(id);
        if (image is null)
            return NotFound();
        return PhysicalFile(image.Address, $"image/{image.Extension}");
    }*/
    
    [HttpGet("")]
    public async Task<IActionResult> GetImagesPaged([FromQuery(Name = "page_number")] 
        [Range(1, int.MaxValue)]
        [Required]
        int pageNumber,
                                                           
        [FromQuery(Name = "page_size")] 
        [Range(1, int.MaxValue)]
        [Required]
        int pageSize)
    {
        var images = await _imageManager.GetAllPaged(pageNumber, pageSize);
        return Ok(images.Select(ResourcesMappers.ToReadImageDTO)
            .ToArray());
    }

    [HttpPost("post")]
    public async Task<IActionResult> PostImage(string tags, string name, Stream uploadedFile)
    {
        var ext = _imageManager.GetExtension(uploadedFile.ToString());
        if (ext is null)
            return BadRequest("unexpected file extension");
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return BadRequest("you are not logged in");
        await _imageManager.Create(user.Id, _address, name, tags, uploadedFile, ext);
        return Ok();
    }
}