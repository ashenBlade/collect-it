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
    public TechSupportConversation? AddClient(string clientId)
    {
        lock (_locker)
        {
            TechSupportConversation? conversation = null;
            var techSupportId = _pendingSupportsIds.FirstOrDefault();
            if (techSupportId is not null)
            {
                _pendingSupportsIds.Remove(techSupportId);
                conversation = new TechSupportConversation()
                               {
                                   ClientId = clientId, 
                                   TechSupportId = techSupportId, 
                                   GroupId = GetGroupId(clientId, techSupportId)
                               };
                _idToSupportConversations.Add(clientId, conversation);
                _idToSupportConversations.Add(techSupportId, conversation);
            }
            else
            {
                _pendingClientsIds.Add(clientId);
            }

            return conversation;
        }
    }


    public TechSupportConversation? AddTechSupport(string techSupportId)
    {
        lock (_locker)
        {
            TechSupportConversation? conversation = null;
            var clientId = _pendingClientsIds.FirstOrDefault();
            if (clientId is not null)
            {
                _pendingClientsIds.Remove(clientId);
                conversation = new TechSupportConversation()
                               {
                                   ClientId = clientId, 
                                   TechSupportId = techSupportId, 
                                   GroupId = GetGroupId(clientId, techSupportId)
                               };
                _idToSupportConversations.Add(techSupportId, conversation);
                _idToSupportConversations.Add(clientId, conversation);
            }
            else
            {
                _pendingSupportsIds.Add(techSupportId);
            }

            return conversation;
        }
    }
    
    public TechSupportConversation? DisconnectUser(string id)
    {
        lock (_locker)
        {
            if (_idToSupportConversations.TryGetValue(id, out var conversation))
            {
                _idToSupportConversations.Remove(conversation.ClientId);
                _idToSupportConversations.Remove(conversation.TechSupportId);
            }
            _pendingClientsIds.Remove(id);
            _pendingSupportsIds.Remove(id);
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