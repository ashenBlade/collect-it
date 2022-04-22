using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Account;
using NpgsqlTypes;

namespace CollectIt.Database.Entities.Resources;

public class Resource
{
    [Key]
    public int Id { get; set; }
        
    // [Required]
    public User Owner { get; set; }

    [Required]
    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }

    [Required]
    public string Name { get; set; }

    public ICollection<User> AcquiredBy { get; set; }
    public NpgsqlTsVector NameSearchVector { get; set; }
    public NpgsqlTsVector TagsSearchVector { get; set; }

    [Required]
    public string FileName { get; set; }
    
    [Required]
    public string Extension { get; set; }
    
    public string[] Tags { get; set; }
    
    [Required]
    public DateTime UploadDate { get; set; }
}