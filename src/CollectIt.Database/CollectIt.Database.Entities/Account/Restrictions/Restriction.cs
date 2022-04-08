using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

public abstract class Restriction
{
    [Key]
    public int Id { get; set; }

    public abstract bool IsSatisfiedBy(Resource resource);
    
    public Subscription Subscription { get; set; }
    public abstract string ErrorMessage { get; }
}