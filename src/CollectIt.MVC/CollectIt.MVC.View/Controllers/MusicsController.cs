using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using CollectIt.MVC.View.DTO;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

[Route("musics")]
public class MusicsController : Controller
{
    private readonly IMusicManager _musicManager;
    private readonly UserManager _userManager;

    public MusicsController(IMusicManager musicManager, UserManager userManager)
    {
        _musicManager = musicManager;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQueriedMusics([FromQuery(Name = "q")] string query, 
                                                      [Range(1, int.MaxValue)]
                                                      [FromQuery(Name = "page_number")]
                                                      int pageNumber,
                                                      [Range(1, int.MaxValue)]
                                                      [FromQuery(Name = "page_size")]
                                                      int pageSize)
    {
        return View("Error", new ErrorViewModel()
                             {
                                 Message = "Videos page is not implemented yet"
                             });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMusicsPageAsync(int id)
    {
        var image = await _musicManager.FindByIdAsync(id);
        if (image is null)
        {
            return NotFound();
        }

        return View("Error", new ErrorViewModel()
                             {
                                 Message = "Videos page is not implemented yet"
                             });
    }

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> UploadNewMusic(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)]
        [Required] 
        CreateMusicDTO dto)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        try
        {
            await using var stream = dto.FormFile.OpenReadStream();
            var music = await _musicManager.CreateAsync(dto.Name, userId, dto.Tags, stream, dto.Extension,
                                                        dto.Duration);

            return CreatedAtAction("GetMusicsPage", new {id = music.Id}, new { });
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    
    [HttpGet("download/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DownloadMusicContent(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (await _musicManager.IsAcquiredBy(id, userId))
        {
            var stream = await _musicManager.GetContentAsync(id);
            return File(stream, "music/*");
        }

        return BadRequest();
    }
}