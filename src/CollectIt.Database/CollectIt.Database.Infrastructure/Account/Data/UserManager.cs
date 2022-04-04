using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class UserManager: UserManager<User>
{
    private readonly PostgresqlCollectItDbContext _context;

    public UserManager(IUserStore<User> store,
                       IOptions<IdentityOptions> optionsAccessor,
                       IPasswordHasher<User> passwordHasher,
                       IEnumerable<IUserValidator<User>> userValidators,
                       IEnumerable<IPasswordValidator<User>> passwordValidators,
                       ILookupNormalizer keyNormalizer,
                       IdentityErrorDescriber errors,
                       IServiceProvider services,
                       ILogger<UserManager> logger,
                       PostgresqlCollectItDbContext context)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
               services, logger)
    {
        _context = context;
    }

    public Task<User?> FindUserByIdAsync(int id)
    {
        return _context.Users
                       .SingleOrDefaultAsync(u => u.Id == id);
    }

    public IAsyncEnumerable<UserSubscription> GetSubscriptionsForUserAsync(User user)
    {
        return _context.UsersSubscriptions
                       .Where(us => us.UserId == user.Id)
                       .Include(us => us.Subscription)
                       .Include(us => us.User)
                       .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<UserSubscription> GetSubscriptionsForUserByIdAsync(int userId)
    {
        return _context.UsersSubscriptions
                       .Where(us => us.UserId == userId)
                       .Include(us => us.Subscription)
                       .Include(us => us.User)
                       .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ActiveUserSubscription> GetActiveSubscriptionsForUserAsync(User user)
    {
        return _context.ActiveUsersSubscriptions
                       .Where(us => us.UserId == user.Id)
                       .Include(us => us.Subscription)
                       .Include(us => us.User)
                       .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ActiveUserSubscription> GetActiveSubscriptionsForUserByIdAsync(int userId)
    {
        return _context.ActiveUsersSubscriptions
                       .Where(aus => aus.UserId == userId)
                       .Include(aus => aus.Subscription)
                       .Include(aus => aus.User)
                       .AsAsyncEnumerable();
    }
}