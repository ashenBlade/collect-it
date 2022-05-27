using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class UserManager : UserManager<User>
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

    public async Task<List<UserSubscription>> GetSubscriptionsForUserByIdAsync(int userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
        {
            throw new UserNotFoundException(userId);
        }

        return await _context.UsersSubscriptions
                             .Where(us => us.UserId == userId)
                             .Include(us => us.Subscription)
                             .ThenInclude(s => s.Restriction)
                             .Include(us => us.User)
                             .ToListAsync();
    }

    public Task<List<AcquiredUserResource>> GetAcquiredResourcesForUserByIdAsync(int userId)
    {
        return _context.AcquiredUserResources
                       .Where(aur => aur.UserId == userId)
                       .Include(aur => aur.Resource)
                       .Include(aur => aur.User)
                       .ToListAsync();
    }

    public Task<List<Resource>> GetUsersResourcesForUserByIdAsync(int userId)
    {
        return _context.Resources
                       .Where(r => r.OwnerId == userId)
                       .Include(r => r.Owner)
                       .ToListAsync();
    }

    public Task<List<ActiveUserSubscription>> GetActiveSubscriptionsForUserByIdAsync(int userId)
    {
        return _context.ActiveUsersSubscriptions
                       .Where(aus => aus.UserId == userId)
                       .Include(aus => aus.Subscription)
                       .ThenInclude(s => s.Restriction)
                       .Include(aus => aus.User)
                       .ToListAsync();
    }

    public Task<List<User>> GetUsersPaged(int pageNumber, int pageSize)
    {
        return _context.Users
                       .Include(u => u.Roles)
                       .OrderBy(u => u.Id)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    public Task<List<Role>> GetRolesAsync(int userId)
    {
        return _context.Users
                       .Where(u => u.Id == userId)
                       .Include(u => u.Roles)
                       .SelectMany(u => u.Roles)
                       .ToListAsync();
    }

    public async Task ChangeUsernameAsync(int userId, string username)
    {
        var user = await FindUserByIdAsync(userId);
        var result = await SetUserNameAsync(user, username);
        if (result.Succeeded)
        {
            return;
        }

        throw new AccountException(result.Errors.Select(err => err.Description).Aggregate((s, n) => $"{s}\n{n}"));
    }

    public Task<List<AcquiredUserResource>> GetAcquiredUserImagesAsync(int userId)
    {
        return _context.AcquiredUserResources
                       .Where(aur => aur.UserId == userId)
                       .Join(_context.Images, aur => aur.ResourceId, i => i.Id, (aur, img) => aur)
                       .Include(aur => aur.Resource)
                       .ToListAsync();
    }
}