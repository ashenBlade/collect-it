using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<Role[]> GetRolesForUserByIdAsync(int userId);
    Task<UserSubscription[]> GetUserSubscriptionsForUserByIdAsync(int userId);
}