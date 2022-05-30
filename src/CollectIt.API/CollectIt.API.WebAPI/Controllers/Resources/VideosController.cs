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

[ApiController]
[Route("api/v1/videos")]
public class VideosController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly IVideoManager _videoManager;

    public VideosController(IVideoManager videoManager, UserManager userManager)
    {
        _videoManager = videoManager;
        _userManager = userManager;
    }

    /// <summary>
    /// Get list of videos
    /// </summary>
    /// <param name="query">Query to search videos</param>
    /// <param name="pageNumber">Number of page to return</param>
    /// <param name="pageSize">Max size of page</param>
    /// <response code="200">Returns list of videos</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadVideoDTO[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVideosPaged([FromQuery(Name = "q")] string? query,
                                                    [Required]
                                                    [FromQuery(Name = "page_number")]
                                                    [Range(1, int.MaxValue)]
                                                    int pageNumber,
                                                    [Required] [FromQuery(Name = "page_size")] [Range(1, int.MaxValue)]
                                                    int pageSize)
    {
        var q = query is null
                    ? await _videoManager.GetPagedAsync(pageNumber, pageSize)
                    : await _videoManager.QueryAsync(query, pageNumber, pageSize);
        return Ok(q.Result.Select(ResourcesMappers.ToReadVideoDTO));
    }

    /// <summary>
    /// Find video by it's id
    /// </summary>
    /// <response code="200">Video found</response>
    /// <response code="404">Video not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadVideoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVideoById([Required] int id)
    {
        var video = await _videoManager.FindByIdAsync(id);
        return video is null
                   ? NotFound()
                   : Ok(ResourcesMappers.ToReadVideoDTO(video));
    }

    /// <summary>
    /// Change video name
    /// </summary>
    /// <response code="404">Video not found</response>
    /// <response code="204">Video's name was changed</response>
    [HttpPost("{id:int}/name")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeVideoName(int id,
                                                     [Required] [FromForm(Name = "Name")] string name)
    {
        try
        {
            await _videoManager.ChangeNameAsync(id, name);
            return NoContent();
        }
        catch (VideoNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change video tags
    /// </summary>
    /// <response code="404">Video not found</response>
    /// <response code="204">Video's tags was changed</response>
    [HttpPost("{id:int}/tags")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeVideoTags(int id,
                                                     [Required] [FromForm(Name = "Tags")] string[] tags)
    {
        try
        {
            await _videoManager.ChangeTagsAsync(id, tags);
            return NoContent();
        }
        catch (VideoNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Create new video
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="400">Invalid values for creation provided</response>
    /// <response code="204">Video was created</response>
    [HttpPost("")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResourcesDTO.CreateVideoDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNewVideo([Required] [FromForm] ResourcesDTO.CreateVideoDTO dto)
    {
        try
        {
            var ownerId = int.Parse(_userManager.GetUserId(User));
            await using var stream = dto.Content.OpenReadStream();
            var video = await _videoManager.CreateAsync(dto.Name,
                                                        ownerId,
                                                        dto.Tags,
                                                        stream,
                                                        dto.Extension,
                                                        dto.Duration);

            return CreatedAtAction("GetVideoById", new {id = video.Id}, ResourcesMappers.ToReadVideoDTO(video));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidResourceCreationValuesException e)
        {
            return BadRequest(new {e.Message});
        }
    }

    /// <summary>
    /// Delete video
    /// </summary>
    /// <response code="204">Video was deleted</response>
    /// <response code="404">Video not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteVideoById(int id)
    {
        try
        {
            await _videoManager.RemoveByIdAsync(id);
            return NoContent();
        }
        catch (VideoNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Download video content
    /// </summary>
    /// <response code="200">Video was found and content returned</response>
    /// <response code="404">Video was not found</response>
    /// <response code="402">Video was not bought by user</response>
    [HttpGet("{id:int}/download")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadVideoContent(int id)
    {
        var file = await _videoManager.FindByIdAsync(id);
        if (file is null)
        {
            return NotFound();
        }

        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _videoManager.IsAcquiredBy(id, userId))
        {
            return StatusCode(StatusCodes.Status402PaymentRequired);
        }

        var content = await _videoManager.GetContentAsync(id);
        return File(content, $"video/{file.Extension}", $"{file.Name}.{file.Extension}");
    }
}