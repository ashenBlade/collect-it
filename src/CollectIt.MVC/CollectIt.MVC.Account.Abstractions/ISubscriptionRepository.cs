using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions;

public interface ISubscriptionRepository : IRepository<Subscription, int>
{ }