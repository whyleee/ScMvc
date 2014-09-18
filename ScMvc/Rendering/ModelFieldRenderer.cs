using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;

namespace ScMvc.Rendering
{
    // TODO: review, and remove "model" overloads here
    public class ModelFieldRenderer : FieldRenderer
    {
        private string fieldValue = string.Empty;

        public string DefaultText { get; set; }

        public string Render(object model, object @params)
        {
            return RenderField(model, @params).ToString();
        }

        public RenderFieldResult RenderField(object model, object @params)
        {
            Item item = this.Item ?? this.GetItem();
            if (item == null || item.Fields[this.FieldName] == null)
            {
                return new RenderFieldResult();
            }
            var modelParams = new RouteValueDictionary(@params);
            //var webeditParams = new SafeDictionary<string>();
            //foreach (var param in modelParams)
            //{
            //    webeditParams.Add(param.Key, param.Value.ToString());
            //}
            var dict = new SafeDictionary<string>();
            foreach (var param in modelParams)
            {
                dict.Add(param.Key, Convert.ToString(param.Value));
            }
            this.Parameters = WebUtil.BuildQueryString(dict, false);
            var renderFieldArgs = new RenderFieldArgs
            {
                After = this.After,
                Before = this.Before,
                EnclosingTag = this.EnclosingTag,
                Item = item,
                FieldName = this.FieldName,
                Parameters = WebUtil.ParseQueryString(this.Parameters),
                RawParameters = this.Parameters,
                RenderParameters = this.RenderParameters,
                DisableWebEdit = this.DisableWebEditing,
                //WebEditParameters = webeditParams
            };
            if (item.Fields[this.FieldName].TypeKey == "multi-line text")
            {
                renderFieldArgs.RenderParameters["linebreaks"] = "<br/>";
            }
            if (renderFieldArgs.Parameters["disable-web-editing"] == "true")
            {
                renderFieldArgs.DisableWebEdit = true;
            }
            if (!string.IsNullOrEmpty(DefaultText))
            {
                renderFieldArgs.RenderParameters.Add("default-text", DefaultText);
            }
            if (!string.IsNullOrEmpty(this.fieldValue))
            {
                typeof(RenderFieldArgs)
                    .GetProperty("FieldValue", BindingFlags.Instance | BindingFlags.Public)
                    .SetValue(renderFieldArgs, this.fieldValue);
            }
            CorePipeline.Run("renderField", renderFieldArgs);
            return renderFieldArgs.Result;
        }

        public void OverrideFieldValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            this.fieldValue = value;
        }
    }
}
