using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ScMvc.Binding;
using ScMvc.Models;
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

            RegisterHtmlStringAwareObjectTemplate();
        }

        #region MVC DisplayFor->IHtmlString fix (hack)

        private static Func<HtmlHelper, string> _defaultObjectTemplate;

        private static void RegisterHtmlStringAwareObjectTemplate()
        {
            var templateHelpers = Type.GetType("System.Web.Mvc.Html.TemplateHelpers, System.Web.Mvc");
            var defTemplates = (Dictionary<string, Func<HtmlHelper, string>>)
                templateHelpers.GetField("_defaultDisplayActions", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            defTemplates[typeof(object).Name] = HtmlStringAwareObjectTemplate;
        }

        private static string HtmlStringAwareObjectTemplate(HtmlHelper html)
        {
            var viewData = html.ViewContext.ViewData;

            if (viewData.Model is IHtmlString)
            {
                return (GetCopyWithParams((IHtmlString)viewData.Model, html.ViewData)).ToHtmlString();
            }

            return DefaultObjectTemplate(html);
        }

        private static T GetCopyWithParams<T>(T value, IDictionary<string, object> @params)
        {
            if (value is ICustomizableModel)
            {
                if (value is ICloneable)
                {
                    value = (T)((ICloneable)value).Clone();
                }

                ((ICustomizableModel)value).Attributes = @params;
            }

            return value;
        }

        private static string DefaultObjectTemplate(HtmlHelper html)
        {
            if (_defaultObjectTemplate != null)
            {
                return _defaultObjectTemplate(html);
            }

            var defTemplatesType = Type.GetType("System.Web.Mvc.Html.DefaultDisplayTemplates, System.Web.Mvc");
            var defObjectTemplateMethod = defTemplatesType.GetMethod("ObjectTemplate",
                BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(HtmlHelper) }, null);

            var call = Expression.Call(defObjectTemplateMethod, Expression.Constant(html));
            var param = Expression.Parameter(typeof(HtmlHelper));
            var lambda = Expression.Lambda<Func<HtmlHelper, string>>(call, param);

            _defaultObjectTemplate = lambda.Compile();

            return _defaultObjectTemplate(html);
        }

        #endregion
    }
}
