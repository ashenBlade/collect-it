using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Entities.Account.Restrictions;

/// <summary>
/// For resources with upload date later than <see cref="DaysAfter"/> days from today
/// The oldest
/// </summary>
public class DaysAfterRestriction : Restriction
{
    [Required]
    public int DaysAfter { get; set; }

    public override bool IsSatisfiedBy(Resource resource)
    {
        return DateTime.Today - TimeSpan.FromDays(DaysAfter) < resource.UploadDate;
    }

    public override string ErrorMessage => "Изображение было загружено позже требуемой ограничением даты";
}