using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions;

public interface IUserRepository : IRepository<User, int>
{ }