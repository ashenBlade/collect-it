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

    public IAsyncEnumerable<UserSubscription> GetActiveSubscriptionsForUserAsync(User user)
    {
        var today = DateTime.Now;
        return _context.UsersSubscriptions
                       .Where(us => us.UserId == user.Id
                                 && us.LeftResourcesCount > 0
                                 && us.During.UpperBound > today
                                 && us.During.LowerBound < today)
                       .AsAsyncEnumerable();
    }
}