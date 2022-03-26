using System.Runtime.InteropServices.ComTypes;
using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Account.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PostgresqlIdentityDbContext _context;

    public SubscriptionRepository(PostgresqlIdentityDbContext context)
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
}