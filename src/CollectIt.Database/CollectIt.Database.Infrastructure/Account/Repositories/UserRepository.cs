using CollectIt.Database.Infrastructure;
using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace CollectIt.MVC.Account.Infrastructure.Repositories;

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