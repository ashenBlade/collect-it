using CollectIt.MVC.Account.IdentityEntities;
using Microsoft.AspNetCore.Identity;
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
        throw new NotImplementedException();
    }

    public async Task RemoveSubscriptionAsync(UserSubscription userSubscription)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserSubscription>> GetSubscriptionsForUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsForUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}