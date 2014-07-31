using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Perks;
using Sitecore.Data.Items;

namespace ScMvc.Binding
{
    public class SitecoreItemModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                return value.RawValue;
            }

            return null;
        }
    }

    public class SitecoreItemModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            if (modelType.Is<Item>())
            {
                return new SitecoreItemModelBinder();
            }

            return null;
        }
    }
}
