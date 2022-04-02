using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface IRestrictionVerifier
{
    public bool IsSatisfiedBy(Resource resource);
}