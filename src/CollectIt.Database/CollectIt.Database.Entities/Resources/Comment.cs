using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Entities.Resources;

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