using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class IndexViewModel
{
    [Required]
    public string Query { get; set; }

    [Required]
    public ResourceType ResourceType { get; set; }
}