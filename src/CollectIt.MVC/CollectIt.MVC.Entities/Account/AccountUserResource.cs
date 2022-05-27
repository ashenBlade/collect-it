using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;

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
    public string Extension { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public ResourceType ResourceType { get; set; }
}