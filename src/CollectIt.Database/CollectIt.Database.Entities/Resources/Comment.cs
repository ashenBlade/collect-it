using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Entities.Resources;

public class Comment
{
    [Key]
    public int CommentId { get; set; }
    
    public int OwnerId { get; set; }
    [Required]
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public DateTime UploadDate { get; set; }
    
    public int TargetId { get; set; }
    [Required]
    [ForeignKey(nameof(TargetId))]
    public Resource Target { get; set; }
}