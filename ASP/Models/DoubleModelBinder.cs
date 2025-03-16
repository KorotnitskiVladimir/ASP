using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP.Models;

public class DoubleModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        throw new NotImplementedException();
    }
}