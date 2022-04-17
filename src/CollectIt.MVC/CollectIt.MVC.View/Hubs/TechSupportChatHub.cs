using Microsoft.AspNetCore.SignalR;

namespace CollectIt.MVC.View.Hubs;

public class TechSupportChatHub : Hub
{
    private readonly ILogger<TechSupportChatHub> _logger;

    public TechSupportChatHub(ILogger<TechSupportChatHub> logger)
    {
        _logger = logger;
    }
}