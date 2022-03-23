using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollectIt.MVC.Account.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostgresqlIdentityDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(PostgresqlIdentityDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<int> AddAsync(User user)
    {
        user = ( await _context.Users.AddAsync(user) ).Entity;
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public Task RemoveAsync(User item)
    {
        _context.Users.Remove(item);
        return _context.SaveChangesAsync();
    }

    public Task<User?> FindByIdAsync(int id)
    {
        return _context.Users.SingleOrDefaultAsync(u => u.Id == id);
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return _context.SaveChangesAsync();
    }
}