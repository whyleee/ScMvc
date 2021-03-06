﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using ScMvc.Aids;
using ScMvc.Models;
using ScMvc.Models.Mappers;
using ScMvc.Rendering;
using Sitecore.Collections;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;

namespace ScMvc
{
    public static class HtmlExtensions
    {
        // TODO: I hate this huge method
        public static IHtmlString Editable<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,
            string fieldName = null, string display = null, IEditableItemModel model = null, object @params = null)
            where TModel : IEditableItemModel
        {
            if (model == null)
            {
                model = html.ViewData.Model;
            }

            object value = expression.Compile()(html.ViewData.Model);

            if (value is DateTime)
            {
                value = new Time((value as DateTime?).Value);
            }

            var isComplexField = !(typeof(TValue).Is<string>() || typeof(TValue).IsPrimitive);

            if (isComplexField && value == null && model.IsEditMode)
            {
                value = Activator.CreateInstance<TValue>();
            }

            value = GetCopyWithParams(value, @params, isComplexField);

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

                    // we can't use DisplayFor for link/image, because then @params are ignored
                    if (value is ICustomizableModel && value is IHtmlString)
                    {
                        return ((IHtmlString) value);
                    }

                    // TODO: 'display' param is only supported for strings for now
                    if (display != null)
                    {
                        return new HtmlString(display);
                    }

                    return html.DisplayFor(expression, template, @params);
                }

                // TODO: 'display' param is only supported for strings for now
                if (display != null)
                {
                    return new HtmlString(display);
                }

                return value != null ? new HtmlString(value.ToString()) : null;
            }

            // TODO: hack for now
            if (model.Item == null)
            {
                return new HtmlString("");
            }

            Ensure.ArgumentNotNull(model.Item, "model.Item");

            var renderFieldName = fieldName.IfNotNullOrEmpty() ?? ExpressionHelper.GetExpressionText(expression);

            if (string.IsNullOrEmpty(renderFieldName) && expression.Body is BinaryExpression)
            {
                renderFieldName = ((BinaryExpression) expression.Body).Left.ToString();
            }

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
                var internalField = SitecoreItemToEditableModelMapper.TryGetInternalField(model.Item, renderFieldName);
                if (internalField == null)
                {
                    throw new ArgumentException(string.Format(
                        "Item '{0}' of template '{1}' doesn't have '{2}' field. Check name spelling or pass 'fieldName' param with correct field name (if view model name isn't equal to item field name).",
                        model.Item.ID, model.Item.TemplateName, renderFieldName
                    ));
                }
                else
                {
                    renderFieldName = internalField.Name;
                }
            }

            var renderer = new FieldRenderer
            {
                Item = model.Item,
                FieldName = renderFieldName,
                //DefaultText = !isComplexField && value != null ? value.ToString() : "[" + renderFieldName + "]"
                //DefaultText = "[" + renderFieldName + "]",
                Parameters = GetParamsQueryString(@params)
            };
            renderer.RenderParameters.Add("default-text", "[" + renderFieldName + "]");

            if (value is IEditableModel)
            {
                ((IEditableModel)value).IsEditMode = true;
            }
            if (value is string && !string.IsNullOrEmpty((string) value) && (string) value != "[Empty]")
            {
                renderer.OverrideFieldValue((string) value);
            }
            if (display != null && display != "[Empty]")
            {
                renderer.OverrideFieldValue(display);
            }

            var output = renderer.Render();
            return new HtmlString(output);
        }

        public static IDisposable WrapIn<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, IRenderToTag>> expression, object @params = null)
        {
            var value = expression.Compile()(html.ViewData.Model);

            if (value == null)
            {
                return null;
            }

            value = GetCopyWithParams(value, @params, isComplexValue: true);
            html.ViewContext.Writer.Write(value.StartTag());

            return new EndTagWriter(html.ViewContext.Writer, value.EndTag());
        }

        private static string GetParamsQueryString(object @params)
        {
            var modelParams = new RouteValueDictionary(@params);

            if (!modelParams.Any())
            {
                return "";
            }

            var dict = new SafeDictionary<string>();

            foreach (var param in modelParams)
            {
                dict.Add(param.Key, Convert.ToString(param.Value));
            }
            
            return WebUtil.BuildQueryString(dict, false);
        }

        private static T GetCopyWithParams<T>(T value, object @params, bool isComplexValue)
        {
            if (isComplexValue && value is ICloneable)
            {
                value = (T) ((ICloneable)value).Clone();
            }

            if (value is ICustomizableModel)
            {
                ((ICustomizableModel)value).Attributes = new RouteValueDictionary(@params);
            }

            return value;
        }

        private sealed class EndTagWriter : IDisposable
        {
            private readonly TextWriter _writer;
            private readonly IHtmlString _endTag;

            public EndTagWriter(TextWriter writer, IHtmlString endTag)
            {
                _writer = writer;
                _endTag = endTag;
            }

            public void Dispose()
            {
                _writer.Write(_endTag);
            }
        }
    }
}
