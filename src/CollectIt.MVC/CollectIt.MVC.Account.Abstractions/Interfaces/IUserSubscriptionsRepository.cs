using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions.Interfaces;

public interface IUserSubscriptionsRepository : IRepository<UserSubscription, int>
{ }