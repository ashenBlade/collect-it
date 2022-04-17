using CollectIt.MVC.Entities.Account.TechSupport;

namespace CollectIt.MVC.Abstractions.TechSupport;

public interface ITechSupportChatManager
{
    public TechSupportConversation? AddClient(string id);
    public TechSupportConversation? DisconnectUser(string id);
    public TechSupportConversation? AddTechSupport(string id);
    public TechSupportConversation? GetConversationByUserId(string id);
}