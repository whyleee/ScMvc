using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScMvc.Models;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering.Pipelines
{
    public class GetLinkFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            if (SkipProcessor(args))
            {
                return;
            }

            var model = ((ModelRenderFieldArgs) args).Model;
            if (!(model is Link))
            {
                return;
            }

            args.DisableWebEditFieldWrapping = true;
            args.DisableWebEditContentEditing = true;

            var html = ((Link) model).ToHtmlString();

            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            args.Result.FirstPart = html;
        }

        protected virtual bool SkipProcessor(RenderFieldArgs args)
        {
            if (!(args is ModelRenderFieldArgs))
            {
                return true;
            }
            var fieldTypeKey = args.FieldTypeKey;
            return fieldTypeKey != "link" && fieldTypeKey != "general link" && fieldTypeKey != "general link with search";
        }
    }
}
