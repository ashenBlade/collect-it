using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

public class TagRestriction : Restriction
{
    [Required]
    public string[] Tags { get; set; }

    public override bool IsSatisfiedBy(Resource resource)
    {
        throw new NotImplementedException("No tags for resource type implemented yet");
    }
}