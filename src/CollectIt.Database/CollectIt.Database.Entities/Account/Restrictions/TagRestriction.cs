using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

public class TagRestriction : Restriction
{
    [Required]
    public string[] Tags { get; set; }

    public override bool IsSatisfiedBy(Resource resource)
    {
        return resource.Tags.Any(tag => Tags.Contains(tag));
    }

    public override string ErrorMessage => "Требуемый ресурс не имеет необходимого тега";
}