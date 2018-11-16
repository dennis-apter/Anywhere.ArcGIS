using Anywhere.ArcGIS.Operation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Anywhere.ArcGIS.Test.AspNetCore.Extensions
{
    public sealed class ArcGISServerOperationBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsAbstract &&
                typeof(ArcGISServerOperation).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new ArcGISServerOperationBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}
