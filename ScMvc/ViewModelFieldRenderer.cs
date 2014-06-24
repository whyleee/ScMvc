using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Perks;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;

namespace ScMvc
{
    public class ViewModelFieldRenderer : FieldRenderer
    {
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
            var webeditParams = new SafeDictionary<string>();
            foreach (var param in modelParams)
            {
                webeditParams.Add(param.Key, param.Value.ToString());
            }
            var renderFieldArgs = new ViewModelRenderFieldArgs
            {
                Model = model,
                After = this.After,
                Before = this.Before,
                EnclosingTag = this.EnclosingTag,
                Item = item,
                FieldName = this.FieldName,
                Parameters = WebUtil.ParseQueryString(this.Parameters),
                RawParameters = this.Parameters,
                RenderParameters = this.RenderParameters,
                DisableWebEdit = this.DisableWebEditing,
                WebEditParameters = webeditParams,
                DefaultText = this.DefaultText // TODO: don't sure this will work without decompiling Sitecore stuff
            };
            if (item.Fields[this.FieldName].TypeKey == "multi-line text")
            {
                renderFieldArgs.RenderParameters["linebreaks"] = "<br/>";
            }
            if (renderFieldArgs.Parameters["disable-web-editing"] == "true")
            {
                renderFieldArgs.DisableWebEdit = true;
            }
            if (DefaultText.IsNotNullOrEmpty())
            {
                renderFieldArgs.RenderParameters.Add("default-text", DefaultText);
            }
            //if (!string.IsNullOrEmpty(this.fieldValue))
            //{
            //    renderFieldArgs.FieldValue = this.fieldValue;
            //}
            CorePipeline.Run("renderField", renderFieldArgs);
            return renderFieldArgs.Result;
        }
    }
}
