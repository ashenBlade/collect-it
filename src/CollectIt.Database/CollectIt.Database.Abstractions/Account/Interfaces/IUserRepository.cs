using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<Role[]> GetRolesForUserByIdAsync(int userId);
    Task<UserSubscription[]> GetUserSubscriptionsForUserByIdAsync(int userId);
}