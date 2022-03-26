using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectIt.Database.Entities.Resources;

[Table("Images")]
public class Image : Resource
{
    public int Id { get; set; }
}