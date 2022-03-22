using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Account.Infrastructure.Repositories;

public class UserSubscriptionsRepository : IUserSubscriptionsRepository
{
    private readonly PostgresqlIdentityDbContext _context;

    public UserSubscriptionsRepository(PostgresqlIdentityDbContext context)
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