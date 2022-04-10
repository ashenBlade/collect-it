using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class CommentViewModel
{
    [Required]
    public string Author { get; set; }
    
    public string[] Comments { get; set; }
}