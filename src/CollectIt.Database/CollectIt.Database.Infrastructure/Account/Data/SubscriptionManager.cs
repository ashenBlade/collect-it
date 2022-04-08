using CollectIt.Database.Abstractions.Account.Exceptions;
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
                                                            int? restrictionId,
                                                            bool active = false)
    {
        var subscription = new Subscription()
                           {
                               Name = name,
                               Description = description,
                               MonthDuration = monthDuration,
                               MaxResourcesCount = maxResourcesCount,
                               RestrictionId = restrictionId,
                               Active = true
                           };
        var result = await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New subscription added: Id = {Id}, Name = {Name}", subscription.Id, subscription.Name);
        return result.Entity;
    }

    public Task<List<Subscription>> GetSubscriptionsAsync(int pageNumber, int pageSize)
    {
        return _context.Subscriptions
                       .OrderBy(s => s.Id)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    public Task<List<Subscription>> GetActiveSubscriptionsAsync(int pageNumber, int pageSize)
    {
        return ActiveSubscriptions
                       .OrderBy(s => s.Id)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    private IQueryable<Subscription> ActiveSubscriptions
    {
        get
        {
            return _context.Subscriptions
                           .Where(s => s.Active);
        }
    }

    public Task<List<Subscription>> GetActiveSubscriptionsAsync()
    {
        return _context.Subscriptions
                       .Where(s => s.Active)
                       .ToListAsync();
    }

    public async Task DeleteSubscriptionAsync(int id)
    {
        var subscription = new Subscription() {Id = id};
        _context.Subscriptions.Attach(subscription);
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Subscription with Id = {SubscriptionId} was deleted", id);
    }
    
    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType)
    {
        return _context.Subscriptions
                       .Include(s => s.Restriction)
                       .Where(s => s.AppliedResourceType == resourceType)
                       .ToListAsync();
    }

    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType, int pageNumber, int pageSize)
    {
        return ActiveSubscriptions
                        .Include(s => s.Restriction)
                       .Where(s => s.AppliedResourceType == resourceType)
                       .OrderBy(s => s.Id)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    public Task ActivateSubscriptionAsync(int subscriptionId)
    {
        return SetActiveSubscriptionState(subscriptionId, true);
    }

    public Task DeactivateSubscriptionAsync(int subscriptionId)
    {
        return SetActiveSubscriptionState(subscriptionId, false);
    }

    private async Task SetActiveSubscriptionState(int subscriptionId, bool activeState)
    {
        var subscription = await _context.Subscriptions
                                         .SingleOrDefaultAsync(s => s.Id == subscriptionId);
        if (subscription is null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        if (subscription.Active == activeState)
            return;

        subscription.Active = activeState;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }

    public Task<Subscription?> FindSubscriptionByIdAsync(int id)
    {
        return _context.Subscriptions
                       .Include(s => s.Restriction)
                       .Where(s => s.Id == id)
                       .SingleOrDefaultAsync();
    }
}