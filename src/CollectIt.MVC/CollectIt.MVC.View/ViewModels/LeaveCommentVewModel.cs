using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class LeaveCommentVewModel
{
    [Required]
    public int ImageId { get; set; }
    [Required]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; }
}