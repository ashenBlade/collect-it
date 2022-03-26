using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IResourceRepository : IRepository<Resource, int>
{ }