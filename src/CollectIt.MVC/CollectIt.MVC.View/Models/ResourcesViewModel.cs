using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.Models;

public class ResourcesViewModel
{
    public IReadOnlyList<Resource> Resources { get; set; }
}