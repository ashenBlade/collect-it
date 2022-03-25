using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.Models;

namespace CollectIt.MVC.View.Views.Shared.Components.ResourceCards;

public class ResourceCardsViewModel
{
    public IReadOnlyList<Resource> Resources { get; set; }
}