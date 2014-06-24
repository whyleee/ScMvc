using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Perks;
using Perks.Mvc;

namespace ScMvc
{
    public static class HtmlExtensions
    {
        public static IHtmlString Editable<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string fieldName = null, string use = null, IEditableModel model = null, object @params = null)
            where TModel : IEditableModel
        {
            if (model == null)
            {
                model = html.ViewData.Model;
            }

            var value = expression.Compile()(html.ViewData.Model);
            var isComplexField = !(typeof(TValue).Is<string>() || typeof(TValue).IsPrimitive);

            if (isComplexField && value == null && model.IsEditMode)
            {
                value = Activator.CreateInstance<TValue>();
            }

            SetParams(value, @params);

            if (!model.IsEditMode)
            {
                if (isComplexField)
                {
                    string template = null;
                    if (value is ITemplatedModel)
                    {
                        template = ((ITemplatedModel)value).Template;
                    }
                    else if (value == null)
                    {
                        object parsedTemplate;
                        if (new RouteValueDictionary(@params).TryGetValue("template", out parsedTemplate))
                        {
                            template = parsedTemplate as string;
                        }
                    }

                    return html.DisplayFor(expression, template, @params);
                }

                // TODO: 'use' param is only supported for strings for now
                if (use != null)
                {
                    return use.ToHtml();
                }

                return value != null ? value.ToString().ToHtml() : null;
            }

            // TODO: hack for now
            if (model.Item == null)
            {
                return new HtmlString("");
            }

            Ensure.ArgumentNotNull(model.Item, "model.Item");

            var renderFieldName = fieldName.IfNotNullOrEmpty() ?? ExpressionHelper.GetExpressionText(expression);
            if (renderFieldName.Contains('.'))
            {
                renderFieldName = renderFieldName.Substring(renderFieldName.LastIndexOf('.') + 1);
            }
            // TODO: depends on 'ContentDictionary'
            //if (model.Item.Is<ContentDictionary>())
            //{
            //    renderFieldName = "Dict_" + renderFieldName;
            //}

            if (model.Item.Fields[renderFieldName] == null)
            {
                throw new ArgumentException(string.Format(
                    "Item '{0}' of template '{1}' doesn't have '{2}' field. Check name spelling or pass 'fieldName' param with correct field name (if view model name isn't equal to item field name).",
                    model.Item.ID, model.Item.TemplateName, renderFieldName
                ));
            }

            var renderer = new ViewModelFieldRenderer
            {
                Item = model.Item,
                FieldName = renderFieldName,
                //DefaultText = !isComplexField && value != null ? value.ToString() : "[" + renderFieldName + "]"
                DefaultText = "[" + renderFieldName + "]"
            };

            if (value is IEditableModel)
            {
                ((IEditableModel)value).IsEditMode = true;
            }
            if (use != null)
            {
                renderer.OverrideFieldValue(use);
            }

            var output = renderer.Render(value, @params);
            return output.ToHtml();
        }

        private static void SetParams(object value, object @params)
        {
            if (value is ICustomizableModel)
            {
                ((ICustomizableModel)value).Attributes = new RouteValueDictionary(@params);
            }
        }
    }
}
