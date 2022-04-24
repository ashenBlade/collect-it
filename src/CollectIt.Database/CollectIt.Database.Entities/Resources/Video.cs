using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.Database.Entities.Resources;

[Table("Videos")]
public class Video : Resource
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Duration { get; set; }
}