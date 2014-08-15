using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScMvc.Models;
using ScMvc.Models.Mappers;
using Sitecore.Data.Fields;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering.Pipelines
{
    public class GetImageFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "image")
            {
                return;
            }

            Image model;

            if (args is ModelRenderFieldArgs)
            {
                model = (Image) ((ModelRenderFieldArgs) args).Model;
            }
            else
            {
                var field = new ImageField(args.GetField(), args.FieldValue);;
                model = new ImageFieldToModelMapper().ToModel(field);
            }

            if (model == null)
            {
                return;
            }

            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;
            args.WebEditClick = "return Sitecore.WebEdit.editControl($JavascriptParameters, 'webedit:chooseimage')";

            var html = model.ToHtmlString();

            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            args.Result.FirstPart = html;
        }
    }
}
