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
/// Manage music in system
/// </summary>
[ApiController]
[Route("api/music")]
public class MusicController : Controller
{
    private IMusicManager _musicManager;
    private UserManager _userManager;

    public MusicController(IMusicManager musicManager, UserManager userManager)
    {
        _musicManager = musicManager;

        _userManager = userManager;
    }


    /// <summary>
    /// Find music by id
    /// </summary>
    /// <response code="404">Music not found</response>
    /// <response code="200">Music found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadMusicDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindMusicById(int id)
    {
        var music = await _musicManager.FindByIdAsync(id);
        if (music is null)
            return NotFound();
        return Ok(ResourcesMappers.ToReadMusicDTO(music));
    }

    /// <summary>
    /// Get list of music
    /// </summary>
    /// <param name="query">Query to search music</param>
    /// <param name="pageNumber">Number of page to return</param>
    /// <param name="pageSize">Max size of page</param>
    /// <response code="200">Returns list of music</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ResourcesDTO.ReadMusicDTO[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMusicPaged([FromQuery(Name = "q")] string? query,
                                                   [Required] [FromQuery(Name = "page_number")] [Range(1, int.MaxValue)]
                                                   int pageNumber,
                                                   [Required] [FromQuery(Name = "page_size")] [Range(1, int.MaxValue)]
                                                   int pageSize)
    {
        var q = query is null
                    ? await _musicManager.GetAllPagedAsync(pageNumber, pageSize)
                    : await _musicManager.QueryAsync(query, pageNumber, pageSize);
        return Ok(q.Result.Select(ResourcesMappers.ToReadMusicDTO));
    }

    /// <summary>
    /// Create new music
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="400">Invalid values for creation provided</response>
    /// <response code="204">Music was created</response>
    [HttpPost("")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResourcesDTO.CreateMusicDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNewMusic([Required] [FromForm] ResourcesDTO.CreateMusicDTO dto)
    {
        try
        {
            var ownerId = int.Parse(_userManager.GetUserId(User));
            await using var stream = dto.Content.OpenReadStream();
            var music = await _musicManager.CreateAsync(dto.Name,
                                                        ownerId,
                                                        dto.Tags,
                                                        stream,
                                                        dto.Extension,
                                                        dto.Duration);

            return CreatedAtAction("FindMusicById", new {id = music.Id}, ResourcesMappers.ToReadMusicDTO(music));
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
    /// Delete music
    /// </summary>
    /// <response code="404">Music not found</response>
    /// <response code="204">Music was deleted</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteMusicById(int id)
    {
        try
        {
            await _musicManager.RemoveByIdAsync(id);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change music name
    /// </summary>
    /// <response code="404">Music not found</response>
    /// <response code="204">Music's name was changed</response>
    [HttpPost("{id:int}/name")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeMusicName(int id,
                                                     [Required] [FromForm(Name = "Name")] string name)
    {
        try
        {
            await _musicManager.ChangeNameAsync(id, name);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change music tags
    /// </summary>
    /// <response code="404">Music not found</response>
    /// <response code="204">Music's tags were changed</response>
    [HttpPost("{id:int}/tags")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeMusicTags(int id,
                                                     [Required] [FromForm(Name = "Tags")] string[] tags)
    {
        try
        {
            await _musicManager.ChangeTagsAsync(id, tags);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }


    /// <summary>
    /// Download music content
    /// </summary>
    /// <response code="200">Music was found and content returned</response>
    /// <response code="404">Music was not found</response>
    /// <response code="402">Music was not bought by user</response>
    [HttpGet("{id:int}/download")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadMusicContent(int id)
    {
        var file = await _musicManager.FindByIdAsync(id);
        if (file is null)
        {
            return NotFound();
        }

        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _musicManager.IsAcquiredBy(id, userId))
        {
            return StatusCode(StatusCodes.Status402PaymentRequired);
        }

        var content = await _musicManager.GetContentAsync(id);
        return File(content, $"audio/{file.Extension}", $"{file.Name}.{file.Extension}");
    }
}