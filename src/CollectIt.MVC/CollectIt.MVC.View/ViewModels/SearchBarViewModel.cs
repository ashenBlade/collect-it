using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class SearchBarViewModel
{
    public string Query { get; set; }
    public ResourceType ResourceType { get; set; }
    public string Controller { get; set; } = "Home";
    public string Action { get; set; } = "Index";
}