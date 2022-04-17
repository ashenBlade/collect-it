using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class ImageViewModel
{
    public int ImageId { get; set; }
    public User Owner { get; set; }
    public bool IsAcquired { get; set; }
    public DateTime UploadDate { get; set; }

    public string Path { get; set; }
    
    public string[] Tags { get; set; }
    
    public IEnumerable<CommentViewModel> Comments { get; set; }
}
