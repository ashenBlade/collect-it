using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account.Restrictions;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Infrastructure.Account;

public class RestrictionVerifier : IRestrictionVerifier
{
    private readonly Restriction? _restriction;

    public RestrictionVerifier(Restriction? restriction)
    {
        _restriction = restriction;
    }
    
    public bool IsSatisfiedBy(Resource resource)
    {
        return _restriction?.IsSatisfiedBy(resource) ?? true;
    }
}