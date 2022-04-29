using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;

namespace CollectIt.MVC.View.Views.Shared.Components.ImageCards;

public class ImageCardsViewModel
{
    public IReadOnlyList<ImageViewModel> Images { get; set; }
    public int PageNumber { get; set; }
    public int MaxPagesCount { get; set; }
    public string Query { get; set; }
}