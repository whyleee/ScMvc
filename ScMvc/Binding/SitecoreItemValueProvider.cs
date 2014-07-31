using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Perks;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace ScMvc.Binding
{
    public class SitecoreItemValueProvider : IValueProvider
    {
        public bool ContainsPrefix(string prefix)
        {
            return prefix == "item";
        }

        public ValueProviderResult GetValue(string key)
        {
            if (!ContainsPrefix(key))
            {
                return null;
            }

            var rendering = RenderingContext.CurrentOrNull.IfNotNull(x => x.Rendering);
            var item = GetDataSourceOrContextItem(rendering);

            return new ValueProviderResult(
                rawValue: item,
                attemptedValue: null,
                culture: CultureInfo.CurrentCulture
            );
        }

        private Item GetDataSourceOrContextItem(Sitecore.Mvc.Presentation.Rendering rendering)
        {
            if (rendering != null)
            {
                return GetDataSourceItem(rendering);
            }
            else
            {
                return Sitecore.Context.Item;
            }
        }

        private Item GetDataSourceItem(Sitecore.Mvc.Presentation.Rendering rendering)
        {
            var dataSource = rendering.DataSource;

            if (dataSource.IsNullOrEmpty())
            {
                return null;
            }

            return Sitecore.Context.Database.GetItem(dataSource);
        }
    }

    public class SitecoreItemValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new SitecoreItemValueProvider();
        }
    }
}
