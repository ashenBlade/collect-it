using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.Models;

public class CommentViewModel
{
    [Required]
    public User Owner { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public DateTime UploadDate { get; set; }
}