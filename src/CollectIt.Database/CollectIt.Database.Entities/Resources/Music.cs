using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.Database.Entities.Resources;


[Table("Musics")]
public class Music : Resource
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Duration { get; set; }
}