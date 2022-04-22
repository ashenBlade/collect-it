using System.ComponentModel.DataAnnotations;
using System.Text;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.Controllers.Resources;

[ApiController]
[Route("api/images")]
public class ImageController : Controller
{
    private IImageManager _imageManager;

    public ImageController(IImageManager imageManager)
    {
        _imageManager = imageManager;
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
}