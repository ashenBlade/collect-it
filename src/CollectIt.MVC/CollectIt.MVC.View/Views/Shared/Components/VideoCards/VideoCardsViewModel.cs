using CollectIt.MVC.View.ViewModels;

namespace CollectIt.MVC.View.Views.Shared.Components.VideoCards;

public class VideoCardsViewModel
{
    public IReadOnlyList<VideoViewModel> Videos { get; set; }
    public int PageNumber { get; set; }
    public int MaxVideosCount { get; set; }
    public string Query { get; set; }
}