using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.Views.Shared.Components.ImageCards;

public class ImageCardsViewModel
{
    public IReadOnlyList<Image> Images { get; set; }
    public int PageNumber { get; set; }
    public int MaxImagesCount { get; set; }
    public string Query { get; set; }
}