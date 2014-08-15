using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScMvc.Aids;
using Sitecore.Data.Items;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering.Pipelines
{
    public class GetDropLinkFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "droplink")
            {
                return;
            }

            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;

            args.Result.FirstPart = GetSelectHtml(args);
        }

        private string GetSelectHtml(RenderFieldArgs args)
        {
            var field = args.GetField();
            var html = new StringBuilder();

            html.AppendFormat("<select id=\"{0}\" class=\"scEnabledChrome\" sc-part-of=\"field\" onchange=\"Sitecore.PageModes.PageEditor.setModified(true)\">",
                SitecoreUtil.GetWebEditingControlId(field)
            );

            var language = Sitecore.Context.ContentLanguage;
            var database = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            var rootPath = field.Source;

            var rootItem = database.GetItem(rootPath, language);
            var selectedOption = args.Item[args.FieldName];

            foreach (Item option in rootItem.GetChildren())
            {
                var displayText = option.DisplayName.IfNotNullOrEmpty() ?? option.Name;
                var isSelected = option.ID.ToString() == selectedOption;

                html.AppendFormat("<option value=\"{0}\"{1}>{2}", option.ID, isSelected ? " selected" : "", displayText);
            }

            html.Append("</select>");

            return html.ToString();
        }
    }
}
