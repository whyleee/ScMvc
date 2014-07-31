using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ScMvc.Binding;
using ScMvc.Rendering;

namespace ScMvc
{
    public static class Initializer
    {
        public static void RegisterInMvc()
        {
            ValueProviderFactories.Factories.Add(new SitecoreItemValueProviderFactory());
            ModelBinderProviders.BinderProviders.Add(new SitecoreItemModelBinderProvider());
            GlobalFilters.Filters.Add(new FillEmptyModelFieldsInPageEditorActionFilter());
        }
    }
}
