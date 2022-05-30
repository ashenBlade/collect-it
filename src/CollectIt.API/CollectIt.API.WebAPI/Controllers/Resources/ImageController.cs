using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Abstractions.Resources.Exceptions;
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

    public ImageController(IImageManager imageManager, UserManager userManager)
    {
        _imageManager = imageManager;
        _userManager = userManager;
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

    /// <summary>
    /// Get list of images
    /// </summary>
    /// <param name="query">Query to search images</param>
    /// <param name="pageNumber">Number of page to return</param>
    /// <param name="pageSize">Max size of page</param>
    /// <response code="200">Returns list of images</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadImageDTO[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImagesPaged([FromQuery(Name = "q")] [MaxLength(100)] string? query,
                                                    [Required]
                                                    [FromQuery(Name = "page_number")]
                                                    [Range(1, int.MaxValue)]
                                                    int pageNumber,
                                                    [Required] [FromQuery(Name = "page_size")] [Range(1, int.MaxValue)]
                                                    int pageSize)
    {
        var q = query is null
                    ? await _imageManager.GetAllPagedAsync(pageNumber, pageSize)
                    : await _imageManager.QueryAsync(query, pageNumber, pageSize);
        return Ok(q.Result.Select(ResourcesMappers.ToReadImageDTO));
    }

    /// <summary>
    /// Create new image
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="400">Invalid values for creation provided</response>
    /// <response code="204">Image was created</response>
    [HttpPost("")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResourcesDTO.CreateImageDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNewImage([Required] [FromForm] ResourcesDTO.CreateImageDTO dto)
    {
        try
        {
            var ownerId = int.Parse(_userManager.GetUserId(User));
            await using var stream = dto.Content.OpenReadStream();
            var image = await _imageManager.CreateAsync(dto.Name,
                                                        ownerId,
                                                        dto.Tags,
                                                        stream,
                                                        dto.Extension);

            return CreatedAtAction("FindImageById", new {id = image.Id}, ResourcesMappers.ToReadImageDTO(image));
        }
        catch (UserNotFoundException)
        {
            return NotFound(new {Message = "User with provided id was not found"});
        }
        catch (InvalidResourceCreationValuesException e)
        {
            return BadRequest(new {e.Message});
        }
    }

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
            await _imageManager.RemoveByIdAsync(id);
            return NoContent();
        }
        catch (ImageNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change image name
    /// </summary>
    /// <response code="404">Image not found</response>
    /// <response code="204">Image's name was changed</response>
    [HttpPost("{id:int}/name")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeImageName(int id,
                                                     [Required] [FromForm(Name = "Name")] string name)
    {
        try
        {
            await _imageManager.ChangeNameAsync(id, name);
            return NoContent();
        }
        catch (ImageNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change image tags
    /// </summary>
    /// <response code="404">Image not found</response>
    /// <response code="204">Image's tags were changed</response>
    [HttpPost("{id:int}/tags")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeImageTags(int id,
                                                     [Required] [FromForm(Name = "Tags")] string[] tags)
    {
        try
        {
            await _imageManager.ChangeTagsAsync(id, tags);
            return NoContent();
        }
        catch (ImageNotFoundException)
        {
            return NotFound();
        }
    }


    /// <summary>
    /// Download image content
    /// </summary>
    /// <response code="200">Image was found and content returned</response>
    /// <response code="404">Image was not found</response>
    /// <response code="402">Image was not bought by user</response>
    [HttpGet("{id:int}/download")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadImageContent(int id)
    {
        var file = await _imageManager.FindByIdAsync(id);
        if (file is null)
        {
            return NotFound();
        }

        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _imageManager.IsAcquiredBy(id, userId))
        {
            return StatusCode(StatusCodes.Status402PaymentRequired);
        }

        var content = await _imageManager.GetContentAsync(id);
        return File(content, $"image/{file.Extension}", $"{file.Name}.{file.Extension}");
    }
}