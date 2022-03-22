using CollectIt.MVC.Account.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollectIt.MVC.Account.Infrastructure.Data;

public class UserManager: UserManager<User>
{
    private readonly PostgresqlIdentityDbContext _context;

    public UserManager(IUserStore<User> store,
                       IOptions<IdentityOptions> optionsAccessor,
                       IPasswordHasher<User> passwordHasher,
                       IEnumerable<IUserValidator<User>> userValidators,
                       IEnumerable<IPasswordValidator<User>> passwordValidators,
                       ILookupNormalizer keyNormalizer,
                       IdentityErrorDescriber errors,
                       IServiceProvider services,
                       ILogger<UserManager> logger,
                       PostgresqlIdentityDbContext context)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
               services, logger)
    {
        _context = context;
    }

    public async Task AddSubscriptionAsync(UserSubscription userSubscription)
    {
        await _context.UsersSubscriptions.AddAsync(userSubscription);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSubscriptionAsync(UserSubscription userSubscription)
    {
        _context.UsersSubscriptions.Remove(userSubscription);
        await _context.SaveChangesAsync();
    }

    public IAsyncEnumerable<UserSubscription> GetSubscriptionsForUserAsync(User user)
    {
        return _context.UsersSubscriptions
                       .Where(us => us.UserId == user.Id)
                       .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ActiveUserSubscription> GetActiveSubscriptionsForUserAsync(User user)
    {
        return _context.ActiveUsersSubscriptions
                       .Where(us => us.UserId == user.Id)
                       .AsAsyncEnumerable();
    }
}