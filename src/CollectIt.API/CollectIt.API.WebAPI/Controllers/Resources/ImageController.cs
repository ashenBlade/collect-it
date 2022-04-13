using System.Text;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Resources;
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
    public  async Task<IActionResult> FindImageById(int id)
    {
        var image = await _imageManager.FindByIdAsync(id);
        if (image is null)
            return NotFound();
        await using (var fileStream = new FileStream(image.Address, FileMode.Open))
        {
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer);
            return Ok(ResourcesMappers.ToReadImageDTO(image, buffer));
        }
    }
}