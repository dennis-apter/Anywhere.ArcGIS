using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Anywhere.ArcGIS.Test.AspNetCore
{
    public class ArcGISServerOperationModelBindingContext : ModelBindingContext
    {
        public ArcGISServerOperationModelBindingContext(string url = null)
        {
            // TODO Init Route $ Body

            var features = new FeatureCollection();
            features.Set<IFormFeature>(new FormFeature(new FormCollection(new Dictionary<string, StringValues>())));

            var requestFeature = new HttpRequestFeature();
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("?"))
                {
                    requestFeature.Path = "/arcgis/rest/services/MapServer";
                    requestFeature.QueryString = url.Substring(1);
                }
                else if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    requestFeature.Path = uri.LocalPath;
                    requestFeature.QueryString = uri.Query;
                }
            }

            features.Set<IHttpRequestFeature>(requestFeature);
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());

            var httpContext = new DefaultHttpContext(features);
            ActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        public override NestedScope EnterNestedScope(ModelMetadata modelMetadata, string fieldName, string modelName, object model)
        {
            ModelMetadata = modelMetadata;
            FieldName = fieldName;
            ModelName = modelName;
            Model = model;

            return default(NestedScope);
        }

        public override NestedScope EnterNestedScope()
        {
            return default(NestedScope);
        }

        protected override void ExitNestedScope()
        {
        }

        public override ActionContext ActionContext { get; set; }
        public override string BinderModelName { get; set; }
        public override BindingSource BindingSource { get; set; }
        public override string FieldName { get; set; }
        public override bool IsTopLevelObject { get; set; }
        public override object Model { get; set; }
        public override ModelMetadata ModelMetadata { get; set; }
        public override string ModelName { get; set; }
        public override ModelStateDictionary ModelState { get; set; }
        public override Func<ModelMetadata, bool> PropertyFilter { get; set; }
        public override ValidationStateDictionary ValidationState { get; set; }
        public override IValueProvider ValueProvider { get; set; }
        public override ModelBindingResult Result { get; set; }
    }
}
