using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.Database.Entities.Resources;


[Table("Musics")]
public class Music : Resource
{
    public int Id { get; set; }
    
    [Required]
    public TimeSpan Duration { get; set; }
}