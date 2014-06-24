using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Perks;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace ScMvc
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

            Item item;
            var renderingContext = RenderingContext.CurrentOrNull;

            if (renderingContext != null)
            {
                item = GetDataSourceItem(renderingContext);
            }
            else
            {
                item = Sitecore.Context.Item;
            }

            if (item == null)
            {
                return null;
            }

            return new ValueProviderResult(
                rawValue: item,
                attemptedValue: null,
                culture: CultureInfo.CurrentCulture
            );
        }

        private Item GetDataSourceItem(RenderingContext renderingContext)
        {
            var dataSource = renderingContext.Rendering.DataSource;

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
