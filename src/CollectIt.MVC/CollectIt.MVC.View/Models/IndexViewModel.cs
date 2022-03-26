using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Models;

public class IndexViewModel
{
    [Required]
    public string Query { get; set; }

    [Required]
    public ResourceType ResourceType { get; set; }
}