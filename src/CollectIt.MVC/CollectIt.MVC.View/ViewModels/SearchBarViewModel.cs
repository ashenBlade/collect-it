using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class SearchBarViewModel
{
    public string Query { get; set; }
    public ResourceType ResourceType { get; set; }
}