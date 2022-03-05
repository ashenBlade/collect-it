using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions;

public interface IUserSubscriptionsRepository : IRepository<UserSubscription, int>
{ }