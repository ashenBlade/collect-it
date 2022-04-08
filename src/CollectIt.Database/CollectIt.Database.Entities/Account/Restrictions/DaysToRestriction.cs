using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;


/// <summary>
/// For resources with upload date earlier than <see cref="DaysTo"/> days from today
/// The freshest
/// </summary>
public class DaysToRestriction : Restriction
{
    [Required]
    public int DaysTo { get; set; }

    public override bool IsSatisfiedBy(Resource resource)
    {
        return DateTime.Today - TimeSpan.FromDays(DaysTo) <= resource.UploadDate;
    }

    public override string ErrorMessage => "Изображение было загружено раньше требуемой ограничением даты";
}