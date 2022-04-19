using CollectIt.MVC.Entities.Account.TechSupport;

namespace CollectIt.MVC.Abstractions.TechSupport;

public interface ITechSupportChatManager
{
    public TechSupportConversation? AddClient(string clientId);
    public TechSupportConversation? DisconnectUser(string id);
    public TechSupportConversation? AddTechSupport(string techSupportId);
    public TechSupportConversation? GetConversationByUserId(string id);
}