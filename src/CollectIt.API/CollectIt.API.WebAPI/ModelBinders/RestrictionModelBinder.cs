using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account.Restrictions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.API.WebAPI.ModelBinders;

public class RestrictionModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var types = context.ValueProvider.GetValue(nameof(RestrictionType));
        if (types.Length is 0)
        {
            context.Result = ModelBindingResult.Success(null);
            return;
        }

        if (types.Length is not 1)
        {
            context.ModelState.AddModelError("RestrictionType", "Multiple restriction types are not allowed");
            context.Result = ModelBindingResult.Failed();
            return;
        }

        if (!int.TryParse(types.FirstValue, out var i))
        {
            context.ModelState.AddModelError("RestrictionType", "Restriction type must be enum integer.");
            context.Result = ModelBindingResult.Failed();
            return;
        }

        RestrictionType restrictionType;
        try
        {
            restrictionType = ( RestrictionType ) i;
        }
        catch (Exception)
        {
            context.Result = ModelBindingResult.Failed();
            return;
        }

        switch (restrictionType)
        {
            case RestrictionType.Author:
                int authorId;
                try
                {
                    authorId = int.Parse(context.ValueProvider.GetValue("AuthorId").FirstValue);
                }
                catch (Exception)
                {
                    context.ModelState.AddModelError("AuthorId", "Could not get author id for restriction");
                    context.Result = ModelBindingResult.Failed();
                    return;
                }

                var authorRestrictionDTO = new AccountDTO.CreateAuthorRestrictionDTO(authorId);

                context.Result = ModelBindingResult.Success(authorRestrictionDTO);
                break;
            case RestrictionType.DaysTo:
                int daysTo;
                try
                {
                    daysTo = int.Parse(context.ValueProvider.GetValue("DaysTo").FirstValue);
                }
                catch (Exception)
                {
                    context.ModelState.AddModelError("DaysTo", "Could not get amount of days for restriction");
                    context.Result = ModelBindingResult.Failed();
                    return;
                }

                var daysToRestrictionDTO = new AccountDTO.CreateDaysToRestrictionDTO(daysTo);

                context.Result = ModelBindingResult.Success(daysToRestrictionDTO);
                break;
            case RestrictionType.DaysAfter:
                int daysAfter;
                try
                {
                    daysAfter = int.Parse(context.ValueProvider.GetValue("DaysAfter").FirstValue);
                }
                catch (Exception)
                {
                    context.ModelState.AddModelError("DaysAfter", "Could not get amount of days after restriction");
                    context.Result = ModelBindingResult.Failed();
                    return;
                }

                var daysAfterRestrictionDTO = new AccountDTO.CreateDaysAfterRestrictionDTO(daysAfter);

                context.Result = ModelBindingResult.Success(daysAfterRestrictionDTO);
                break;
            case RestrictionType.Tags:
                string[] tags;
                try
                {
                    tags = context.ValueProvider
                                  .GetValue("Tags")
                                  .SelectMany(str => str.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                  .ToArray();
                    if (tags.Length == 0)
                    {
                        context.ModelState.AddModelError("Tags", "At least 1 tag must be specified");
                        return;
                    }
                }
                catch (Exception)
                {
                    context.ModelState.AddModelError("Tags", "Could not get tags for restriction");
                    context.Result = ModelBindingResult.Failed();
                    return;
                }

                var tagsRestrictionDTO = new AccountDTO.CreateTagsRestrictionDTO(tags);
                context.Result = ModelBindingResult.Success(tagsRestrictionDTO);
                break;
            default:
                context.ModelState.AddModelError("RestrictionType",
                                                 $"Restriction type '{( int ) restrictionType}' is not supported");
                context.Result = ModelBindingResult.Success(new AccountDTO.CreateRestrictionDTO(restrictionType));
                break;
        }
    }
}