using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.ViewModels;

public class ImagesViewModel
{
    public IReadOnlyList<Image> Images { get; set; }
}