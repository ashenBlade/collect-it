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
        catch (Exception e)
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
                    catch (Exception e)
                    {
                        context.ModelState.AddModelError("AuthorId", "Could not get author id for restriction");
                        context.Result = ModelBindingResult.Failed();
                        return;
                    }
                    var authorRestrictionDTO = new AccountDTO.CreateAuthorRestrictionDTO(authorId);
                    
                    context.Result = ModelBindingResult.Success(authorRestrictionDTO);
                    break;
                default:
                    context.ModelState.AddModelError("RestrictionType", $"Restriction type '{restrictionType.ToString()}' is not supported");
                    context.Result = ModelBindingResult.Success(new AccountDTO.CreateRestrictionDTO(restrictionType));
                    break;
            }
    }
}