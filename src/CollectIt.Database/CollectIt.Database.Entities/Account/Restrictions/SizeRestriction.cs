using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

public class SizeRestriction : Restriction
{
    [Required]
    public int SizeBytes { get; set; }
    public override bool IsSatisfiedBy(Resource resource)
    {
        throw new NotImplementedException("Size of resource is not specified yet");
    }

    public override string ErrorMessage => "Размер изображения не соответствует требованию";
}