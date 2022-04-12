using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.Entities.Account;

public class AccountUserResource
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FileName { get; set; }
    
    [Required]
    public string Address { get; set; }

    [Required]
    public string[] Tags { get; set; }
    
    [Required]
    public DateTime AcquireDate { get; set; }
}