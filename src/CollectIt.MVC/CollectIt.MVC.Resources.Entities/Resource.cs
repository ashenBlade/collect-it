using System.ComponentModel.DataAnnotations;
using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Resources.Entities;

public class Resource
{
    [Key]
    public int ResourceId { get; set; }
        
    [Required]
    public User ResourceOwner { get; set; }
        
    [Required]
    public string ResourcePath { get; set; }
        
    [Required]
    public DateTime UploadDate { get; set; }
}