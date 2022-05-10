using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using CollectIt.MVC.View.Views.Shared.Components.MusicCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("musics")]
public class MusicsController : Controller
{
    private const int DefaultPageSize = 15;

    private static readonly HashSet<string> SupportedMusicExtensions = new() {"mp3", "ogg", "wav"};
    private readonly ICommentManager _commentManager;
    private readonly IMusicManager _musicManager;
    private readonly UserManager _userManager;

    public MusicsController(IMusicManager musicManager, UserManager userManager, ICommentManager commentManager)
    {
        _musicManager = musicManager;
        _userManager = userManager;
        _commentManager = commentManager;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Music(int id)
    {
        var source = await _musicManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var user = await _userManager.GetUserAsync(User);
        var model = new MusicViewModel()
                    {
                        MusicId = id,
                        Name = source.Name,
                        OwnerName = source.Owner.UserName,
                        UploadDate = source.UploadDate,
                        BlobAddress = Url.Action("GetMusicBlob", new {id = id})!,
                        DownloadAddress = Url.Action("DownloadMusicContent", new {id = id})!,
                        Tags = source.Tags,
                        IsAcquired = user is not null && await _musicManager.IsAcquiredBy(id, user.Id),
                        Comments = ( await _commentManager.GetResourcesComments(id) ).Select(c => new CommentViewModel()
                                                                                                  {
                                                                                                      Author = c.Owner
                                                                                                                .UserName,
                                                                                                      Comment =
                                                                                                          c.Content,
                                                                                                      PostTime = c
                                                                                                         .UploadDate
                                                                                                  })
                    };
        return View(model);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQueriedMusics([FromQuery(Name = "q")] [Required] string? query,
                                                      [Range(1, int.MaxValue)] [FromQuery(Name = "p")]
                                                      int pageNumber = 1)
    {
        var musics = query is null
                         ? await _musicManager.GetAllPagedAsync(pageNumber, DefaultPageSize)
                         : await _musicManager.QueryAsync(query, pageNumber, DefaultPageSize);
        return View("Musics",
                    new MusicCardsViewModel()
                    {
                        Musics = musics.Result.Select(m => new MusicViewModel()
                                                           {
                                                               BlobAddress =
                                                                   Url.Action("GetMusicBlob", new {id = m.Id})!,
                                                               Name = m.Name,
                                                               MusicId = m.Id,
                                                               OwnerName = m.Owner.UserName
                                                           })
                                       .ToList(),
                        Query = query ?? string.Empty,
                        PageNumber = pageNumber,
                        MaxMusicsCount = musics.TotalCount
                    });
    }

    [HttpGet("upload")]
    [Authorize]
    public async Task<IActionResult> UploadMusic()
    {
        return View();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadMusic(
        [FromForm] [Required] UploadMusicViewModel model)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (!TryGetExtension(model.Content.FileName, out var extension))
        {
            // ModelState.AddModelError("FormFile", $"Поддерживаемые расширения музыки: {SupportedVideoFormats.Aggregate((s, n) => $"{s}, {n}")}");
            return View("Error",
                        new ErrorViewModel()
                        {
                            Message =
                                $"Поддерживаемые расширения видео: {SupportedMusicExtensions.Aggregate((s, n) => $"{s}, {n}")}"
                        });
        }

        try
        {
            await using var stream = model.Content.OpenReadStream();
            var music = await _musicManager.CreateAsync(model.Name, userId,
                                                        model.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                                                        stream, extension,
                                                        model.Duration);

            return RedirectToAction("GetQueriedMusics", new {q = ""});
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    private bool TryGetExtension(string filename, out string extension)
    {
        extension = null!;
        if (filename is null)
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var array = filename.Split('.');
        if (array.Length < 2)
        {
            return false;
        }

        return SupportedMusicExtensions.Contains(extension = array[^1].ToLower());
    }

    [HttpGet("{id:int}/blob")]
    public async Task<IActionResult> GetMusicBlob(int id)
    {
        var music = await _musicManager.FindByIdAsync(id);
        if (music is null)
        {
            return View("Error", new ErrorViewModel() {Message = "Music not found"});
        }

        var stream = await _musicManager.GetContentAsync(id);
        return File(stream, $"audio/{music!.Extension}", $"{music.Name}.{music.Extension}");
    }

    [HttpGet("{id:int}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadMusicContent(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _musicManager.IsAcquiredBy(id, userId))
            return StatusCode(StatusCodes.Status402PaymentRequired);
        var image = await _musicManager.FindByIdAsync(id);
        var stream = await _musicManager.GetContentAsync(id);
        return File(stream, $"audio/{image!.Extension}", $"{image.Name}.{image.Extension}");
    }
}