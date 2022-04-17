using CollectIt.MVC.Abstractions.TechSupport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CollectIt.MVC.View.Hubs;

[Authorize]
public class TechSupportChatHub : Hub<ITechSupportHub>
{
    private readonly ILogger<TechSupportChatHub> _logger;
    private readonly ITechSupportChatManager _chatManager;

    public TechSupportChatHub(ILogger<TechSupportChatHub> logger, 
                              ITechSupportChatManager chatManager)
    {
        _logger = logger;
        _chatManager = chatManager;
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var conversation = _chatManager.DisconnectUser(Context.ConnectionId);
        if (conversation is not null)
        {
            Clients.Group(conversation.GroupId).ChatEnded();
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterClient()
    {
        var conversation = _chatManager.AddClient(Context.ConnectionId);
        if (conversation is null)
        {
            return;
        }

        var group = conversation.GroupId;
        await Groups.AddToGroupAsync(conversation.ClientId, group);
        await Groups.AddToGroupAsync(conversation.TechSupportId, group);
        await Clients.Group(group).ChatStarted();
    }

    [Authorize(Roles = "Technical Support")]
    public async Task RegisterSupport()
    { 
        var conversation = _chatManager.AddTechSupport(Context.ConnectionId);
    }

    [HubMethodName("SendMessage")]
    public async Task SendMessageAsync(string message)
    {
        var conversation = _chatManager.GetConversationByUserId(Context.ConnectionId);
        var groupId = conversation.GroupId;
        var requesterId = Context.ConnectionId;
        await Clients.Group(groupId)
                     .SendMessageAsync(requesterId == conversation.ClientId
                                           ? "Client"
                                           : "Support", message);
    }
}