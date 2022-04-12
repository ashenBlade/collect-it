using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.Entities.Account;

public class AccountUserResources
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string FileType { get; set; }
    
    [Required]
    public DateTime AcquireTime { get; set; }
}