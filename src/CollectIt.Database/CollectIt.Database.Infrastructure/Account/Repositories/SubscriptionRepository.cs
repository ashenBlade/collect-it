using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Account.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PostgresqlCollectItDbContext _context;

    public SubscriptionRepository(PostgresqlCollectItDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> AddAsync(Subscription subscription)
    {
        
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        return subscription.Id;
    }

    public Task<Subscription?> FindByIdAsync(int id)
    {
        return _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task UpdateAsync(Subscription item)
    {
        _context.Subscriptions.Update(item);
        return _context.SaveChangesAsync();
    }

    public Task RemoveAsync(Subscription item)
    {
        _context.Subscriptions.Remove(item);
        return _context.SaveChangesAsync();
    }

    public Task<List<Subscription>> GetAllWithResourceType(ResourceType type)
    {
        return _context.Subscriptions.Where(s => s.AppliedResourceType == type).ToListAsync();
    }

    public Task<List<Subscription>> GetAll()
    {
        return _context.Subscriptions.ToListAsync();
    }
}