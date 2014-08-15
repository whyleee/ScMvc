using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScMvc.Aids;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines.RenderField;

namespace ScMvc.Rendering
{
    public class SelectControl
    {
        public string Render(RenderFieldArgs args)
        {
            var field = args.GetField();
            var hierarchical = args.FieldTypeKey == "droptree";

            var language = Sitecore.Context.ContentLanguage;

            if (string.IsNullOrEmpty(language.Name))
            {
                language = Sitecore.Context.Language;
            }

            var database = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            
            var html = new StringBuilder();

            html.AppendFormat("<select id=\"{0}\" class=\"scEnabledChrome\" sc-part-of=\"field\" onchange=\"Sitecore.PageModes.PageEditor.setModified(true)\">",
                SitecoreUtil.GetWebEditingControlId(field)
            );

            var rootPath = GetRootPath(field);
            var rootItem = database.GetItem(rootPath, language);
            var selectedOption = args.Item[args.FieldName];

            RenderOptions(rootItem.GetChildren(), html, field, selectedOption, hierarchical, level: 1);
            html.Append("</select>");

            return html.ToString();
        }

        private void RenderOptions(IEnumerable<Item> options, StringBuilder html, Field field, string selectedOption, bool hierarchical, int level)
        {
            foreach (var option in options)
            {
                var displayText = GetDisplayText(option, field);
                var indent = "";

                for (int i = 1; i < level; i++)
                {
                    indent += "&nbsp;&nbsp;&nbsp;";
                }

                if (hierarchical)
                {
                    var childOptions = option.GetChildren();

                    if (childOptions.Any())
                    {
                        html.AppendFormat("<optgroup label=\"{0}\">", indent + displayText);
                        RenderOptions(childOptions, html, field, selectedOption, hierarchical, level + 1);
                        html.Append("</optgroup>");

                        continue;
                    }
                }

                var isSelected = option.ID.ToString() == selectedOption;
                html.AppendFormat("<option value=\"{0}\"{1}>{2}", option.ID, isSelected ? " selected" : "", indent + displayText);
            }
        }

        private string GetRootPath(Field field)
        {
            var @params = field.Source.Split('&');

            foreach (var param in @params)
            {
                if (param.StartsWith("/"))
                {
                    return param;
                }
                else if (param.StartsWith("DataSource="))
                {
                    return param.Replace("DataSource=", "");
                }
            }

            return null;
        }

        private string GetDisplayText(Item option, Field field)
        {
            string displayText = null;
            var displayFieldName = field.Source.Split('&')
                .FirstOrDefault(param => param.StartsWith("DisplayFieldName="));

            if (!string.IsNullOrEmpty(displayFieldName))
            {
                displayText = option[displayFieldName.Replace("DisplayFieldName=", "")];
            }

            return displayText.IfNotNullOrEmpty() ?? option.DisplayName.IfNotNullOrEmpty() ?? option.Name;
        }
    }
}
