using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.Database.Entities.Resources;

[Table("Videos")]
public class Video : Resource
{
    public int Id { get; set; }
    
    [Required]
    public TimeSpan Duration { get; set; }
}