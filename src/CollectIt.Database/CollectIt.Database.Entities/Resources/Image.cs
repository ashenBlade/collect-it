using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.MVC.Resources.Entities;

public class Image
{
    [Key]
    public int ImageId { get; set; }
        
    [Required]
    public Resource Resource { get; set; }
}