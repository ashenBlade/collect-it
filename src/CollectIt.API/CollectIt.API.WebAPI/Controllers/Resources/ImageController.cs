using System.ComponentModel.DataAnnotations;
using System.Text;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.API.WebAPI.DTO;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace CollectIt.API.WebAPI.Controllers.Resources;

/// <summary>
/// Manage images in system
/// </summary>
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

    /// <summary>
    /// Find image by id
    /// </summary>
    /// <response code="404">Image not found</response>
    /// <response code="200">Image found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadImageDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
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
    
    
    
    /// <summary>
    /// Get images list 
    /// </summary>
    /// <response code="200">Array of images ordered by id with max size of <paramref name="pageSize"/> </response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadImageDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Post Image
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Image was posted</response>
    /// <response code="400">Incorrect data</response>
    [HttpPost("")]
    public async Task<IActionResult> PostImage([FromForm]ResourcesDTO.UploadImageDTO dto)
    {
        try
        {
            if (!TryGetExtension(dto.Content.FileName, out var ext))
                return BadRequest();
            await using var stream = dto.Content.OpenReadStream();
            var image = await _imageManager.Create(
                dto.OwnerId, 
                _address,
                dto.Name, 
                dto.Tags, 
                stream, 
                ext);

            return CreatedAtAction("FindImageById", new {id = image.Id}, ResourcesMappers.ToReadImageDTO(image));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }    
    
    private static bool TryGetExtension(string filename, out string? extension)
    {
        if (filename is null)
        {
            throw new ArgumentNullException(nameof(filename));
        }
        
        extension = null;
        var array = filename.Split('.');
        if (array.Length < 2)
        {
            return false;
        }

        extension = array[^1].ToLower();
        return SupportedImageExtensions.Contains(extension);
    }
    
    private static readonly HashSet<string> SupportedImageExtensions = new() {"png", "jpeg", "jpg", "webp", "bmp"};
    
    
    /// <summary>
    /// Delete image
    /// </summary>
    /// <response code="404">Image not found</response>
    /// <response code="204">Image was deleted</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteImageById(int id)
    {
        try
        {
            await _imageManager.RemoveAsync(id);
            return NoContent();
        }
        catch(ImageNotFoundException)
        {
            return NotFound();
        }
    }
    
    
    /// <summary>
    /// Download Image
    /// </summary>
    /// <response code="404">Image not found</response>
    /// <response code="400">Physical image not found</response>
    /// <response code="402">Payment required</response>
    /// <response code="200">Image was found</response>
    [HttpGet("{id:int}/download")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DownloadImage(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return NotFound();
        }

        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _imageManager.IsAcquiredBy(id, userId))
        {
            return StatusCode(StatusCodes.Status402PaymentRequired);
        }

        var file = new FileInfo(Path.Combine(_address, source.FileName));
        return file.Exists
            ? PhysicalFile(file.FullName, $"image/{source.Extension}", source.FileName)
            : BadRequest(new {Message = "File not found"});
    }
}