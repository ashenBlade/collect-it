using CollectIt.API.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CollectIt.API.WebAPI.ModelBinders;

public class RestrictionModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(AccountDTO.CreateRestrictionDTO)
                   ? new RestrictionModelBinder()
                   : null;
    }
}