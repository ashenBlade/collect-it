using System.ComponentModel.DataAnnotations;
using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Resources.Entities;

public class Comment
{
    [Key]
    public int CommentId { get; set; }
    
    [Required]
    public User Owner { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public DateTime UploadDate { get; set; }
    
    [Required]
    public Resource Target { get; set; }
}