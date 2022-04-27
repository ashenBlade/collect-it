using CollectIt.MVC.View.ViewModels;

namespace CollectIt.MVC.View.Views.Shared.Components.MusicCards;

public class MusicCardsViewModel
{
    public IReadOnlyList<MusicViewModel> Musics { get; set; }
    public int PageNumber { get; set; }
    public int MaxMusicsCount { get; set; }
    public string Query { get; set; }
}