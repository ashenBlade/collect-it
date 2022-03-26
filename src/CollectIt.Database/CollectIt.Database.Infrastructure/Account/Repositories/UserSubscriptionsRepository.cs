using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Account.Repositories;

public class UserSubscriptionsRepository : IUserSubscriptionsRepository
{
    private readonly PostgresqlCollectItDbContext _context;

    public UserSubscriptionsRepository(PostgresqlCollectItDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> AddAsync(UserSubscription item)
    {
         var entity = await _context.UsersSubscriptions.AddAsync(item);
         await _context.SaveChangesAsync();
         return entity.Entity.Id;
    }

    public Task<UserSubscription?> FindByIdAsync(int id)
    {
        return _context.UsersSubscriptions.SingleOrDefaultAsync(us => us.Id == id);
    }

    public Task UpdateAsync(UserSubscription item)
    {
        _context.UsersSubscriptions.Update(item);
        return _context.SaveChangesAsync();
    }

    public Task RemoveAsync(UserSubscription item)
    {
        _context.UsersSubscriptions.Remove(item);
        return _context.SaveChangesAsync();
    }
}