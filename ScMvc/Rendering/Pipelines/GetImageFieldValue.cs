using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScMvc.Models;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering.Pipelines
{
    public class GetImageFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "image" || !(args is ModelRenderFieldArgs))
            {
                return;
            }

            var model = ((ModelRenderFieldArgs) args).Model;
            if (!(model is Image))
            {
                return;
            }

            var html = ((Image) model).ToHtmlString();

            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;
            args.WebEditClick = "return Sitecore.WebEdit.editControl($JavascriptParameters, 'webedit:chooseimage')";

            args.Result.FirstPart = html;
        }
    }
}
