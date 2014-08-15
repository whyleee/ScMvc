using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScMvc.Models;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering.Pipelines
{
    public class GetDateFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            string fieldTypeKey = args.FieldTypeKey;
            if ((fieldTypeKey != "date" && fieldTypeKey != "datetime") || !(args is ModelRenderFieldArgs))
            {
                return;
            }

            var model = ((ModelRenderFieldArgs) args).Model;
            if (!(model is Time))
            {
                return;
            }

            var html = ((Time) model).ToHtmlString();

            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;
            args.WebEditClick = "return Sitecore.WebEdit.editControl($JavascriptParameters,'webedit:editdate');";

            args.Result.FirstPart = html;
        }
    }
}
