namespace CollectIt.MVC.Entities.Account;

public class Subscription
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Name { get; set; }
    public string ResourceType { get; set; }
    public int LeftResourcesCount { get; set; }
}