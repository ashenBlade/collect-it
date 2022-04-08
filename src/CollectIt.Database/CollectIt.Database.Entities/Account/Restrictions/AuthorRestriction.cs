using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

public class AuthorRestriction : Restriction
{
    [Required]
    public int AuthorId { get; set; }
    
    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; }

    public override bool IsSatisfiedBy(Resource resource)
    {
        return resource.OwnerId == AuthorId;
    }

    public override string ErrorMessage => "Автор изображения не является требуемым для данного типа подписки";
}