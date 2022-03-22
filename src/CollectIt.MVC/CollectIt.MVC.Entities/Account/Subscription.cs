namespace CollectIt.MVC.View.Models;

public class Subscription
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Name { get; set; }
    public string ResourceType { get; set; }
    public int LeftResourcesCount { get; set; }
}