namespace CollectIt.MVC.Abstractions.TechSupport;

public interface ITechSupportHub
{
    public Task SendMessageAsync(string user, string message);
    public Task ChatStarted();
    public Task ChatEnded();
}