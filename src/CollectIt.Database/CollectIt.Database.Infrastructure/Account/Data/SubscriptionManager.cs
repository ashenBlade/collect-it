using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly ILogger<SubscriptionManager> _logger;

    public SubscriptionManager(PostgresqlCollectItDbContext context,
                               ILogger<SubscriptionManager> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Subscription> CreateSubscriptionAsync(string name,
                                                      string description,
                                                      int monthDuration,
                                                      ResourceType appliedResourceType,
                                                      int maxResourcesCount,
                                                      int? restrictionId)
    {
        var subscription = new Subscription()
                           {
                               Name = name,
                               Description = description,
                               MonthDuration = monthDuration,
                               MaxResourcesCount = maxResourcesCount,
                               RestrictionId = restrictionId
                           };
        var result = await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New subscription added: Id = {Id}, Name = {Name}", subscription.Id, subscription.Name);
        return result.Entity;
    }

    public async Task DeleteSubscriptionAsync(int id)
    {
        var subscription = new Subscription() {Id = id};
        _context.Subscriptions.Attach(subscription);
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Subscription with Id = {SubscriptionId} was deleted", id);
    }
}