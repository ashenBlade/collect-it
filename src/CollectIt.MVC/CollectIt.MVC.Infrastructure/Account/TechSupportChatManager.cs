using System.Collections.Concurrent;
using CollectIt.MVC.Abstractions.TechSupport;
using CollectIt.MVC.Entities.Account.TechSupport;

namespace CollectIt.MVC.Infrastructure.Account;

public class TechSupportChatManager : ITechSupportChatManager
{
    private readonly object _locker = new();
    private readonly HashSet<string> _pendingClientsIds = new();
    private readonly HashSet<string> _pendingSupportsIds = new();
    private readonly Dictionary<string, TechSupportConversation> _idToSupportConversations = new();

    private static string GetGroupId(string client, string support)
    {
        return $"{client}::{support}";
    }
    public TechSupportConversation? AddClient(string id)
    {
        lock (_locker)
        {
            TechSupportConversation? conversation = null;
            var techSupportId = _pendingSupportsIds.FirstOrDefault();
            if (techSupportId is not null)
            {
                _pendingSupportsIds.Remove(id);
                conversation = new TechSupportConversation()
                               {
                                   ClientId = id, TechSupportId = techSupportId, GroupId = GetGroupId(id, techSupportId)
                               };
                _idToSupportConversations.Add(id, conversation);
                _idToSupportConversations.Add(techSupportId, conversation);
            }
            else
            {
                _pendingClientsIds.Add(id);
            }

            return conversation;
        }
    }


    public TechSupportConversation? AddTechSupport(string id)
    {
        lock (_locker)
        {
            TechSupportConversation? conversation = null;
            var clientId = _pendingClientsIds.FirstOrDefault();
            if (clientId is not null)
            {
                _pendingClientsIds.Remove(id);
                conversation = new TechSupportConversation()
                               {
                                   ClientId = clientId, TechSupportId = id, GroupId = GetGroupId(clientId, id)
                               };
                _idToSupportConversations.Add(id, conversation);
                _idToSupportConversations.Add(clientId, conversation);
            }
            else
            {
                _pendingSupportsIds.Add(id);
            }

            return conversation;
        }
    }
    
    public TechSupportConversation? DisconnectUser(string id)
    {
        lock (_locker)
        {
            if (_idToSupportConversations.Remove(id, out var conversation))
            {
                _idToSupportConversations.Remove(id == conversation.ClientId
                                                     ? conversation.TechSupportId
                                                     : conversation.ClientId);
            }
            else if (_pendingClientsIds.Remove(id))
            {
                
            }
            else if (_pendingSupportsIds.Remove(id))
            {
                
            }
            return conversation;
        }
    }

    public TechSupportConversation? GetConversationByUserId(string id)
    {
        lock (_locker)
        {
            return _idToSupportConversations.TryGetValue(id, out var value)
                       ? value
                       : null;
        }
    }
}