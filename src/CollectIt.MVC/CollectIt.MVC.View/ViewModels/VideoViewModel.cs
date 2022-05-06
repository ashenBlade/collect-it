﻿namespace CollectIt.MVC.View.ViewModels;

public class VideoViewModel
{
    public int VideoId { get; set; }
    public string Name { get; set; }
    public string OwnerName { get; set; }
    public bool IsAcquired { get; set; }
    public DateTime UploadDate { get; set; }

    public string Address { get; set; }
    
    public string[] Tags { get; set; }
    
    public IEnumerable<CommentViewModel> Comments { get; set; }
}