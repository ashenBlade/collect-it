using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class SearchBarViewModel
{
    public string Query { get; set; }
    public ResourceType ResourceType { get; set; }
}