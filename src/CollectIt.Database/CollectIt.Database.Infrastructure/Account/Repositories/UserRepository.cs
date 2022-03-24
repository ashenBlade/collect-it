using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.Extensions.Logging;

namespace CollectIt.Database.Infrastructure.Account.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(PostgresqlCollectItDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public Task<int> AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(User item)
    {
        throw new NotImplementedException();
    }

    public Task<User> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User item)
    {
        throw new NotImplementedException();
    }

    public Task<Role[]> GetRolesForUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserSubscription[]> GetUserSubscriptionsForUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}