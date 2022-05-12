namespace CollectIt.MVC.View.ViewModels;

public class MusicViewModel
{
    public int MusicId { get; set; }
    public string Name { get; set; }
    public string OwnerName { get; set; }
    public bool IsAcquired { get; set; }
    public DateTime UploadDate { get; set; }

    public string DownloadAddress { get; set; }
    public string PreviewAddress { get; set; }

    public string Duration { get; set; }

    public string[] Tags { get; set; }

    public IEnumerable<CommentViewModel> Comments { get; set; }
}