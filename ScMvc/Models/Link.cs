using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Perks;
using Perks.Mvc;

namespace ScMvc.Models
{
    public class Link : IHtmlString, ICustomizableModel, IEditableModel, ITemplatedModel, IRenderToTag, ICloneable
    {
        private IDictionary<string, object> _attributes;

        public Link()
        {
            _attributes = new Dictionary<string, object>();
        }

        private Link(IDictionary<string, object> attributes)
        {
            Ensure.ArgumentNotNull(attributes, "attributes");

            _attributes = attributes;
        }

        public string Url { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }

        public string Target { get; set; }

        public string Class { get; set; }

        public IDictionary<string, object> Attributes
        {
            get { return _attributes; }
            set { _attributes = this.ParseAttributes(value); }
        }

        public bool IsEditMode { set; get; }

        public string Template { get; set; }

        //public EditableTextField EditTextField { get; set; }

        public object Clone()
        {
            return new Link(_attributes)
            {
                Url = Url,
                Text = Text,
                Title = Title,
                Target = Target,
                Class = Class,
                IsEditMode = IsEditMode,
                Template = Template
            };
        }

        public string ToHtmlString()
        {
            return ToHtml(TagRenderMode.Normal);
        }

        public override string ToString()
        {
            return ToHtmlString();
        }

        public IHtmlString StartTag()
        {
            return ToHtml(TagRenderMode.StartTag).ToHtml();
        }

        public IHtmlString EndTag()
        {
            return ToHtml(TagRenderMode.EndTag).ToHtml();
        }

        private string ToHtml(TagRenderMode renderMode)
        {
            if (Url.IsNullOrEmpty() && Text.IsNullOrEmpty())
            {
                return null;
            }

            TagBuilder tag;

            if (Url.IsNotNullOrEmpty() || IsEditMode)
            {
                tag = new TagBuilder("a");
                tag.Attributes.Add("href", Url);

                if (Target.IsNotNullOrEmpty() && Target != "_self")
                {
                    tag.Attributes.Add("target", Target);
                }
            }
            else
            {
                tag = new TagBuilder("span");
            }

            if (renderMode == TagRenderMode.EndTag)
            {
                return tag.ToString(renderMode);
            }

            //if (IsEditMode && EditTextField != null)
            //{
            //    var renderer = new ViewModelFieldRenderer
            //    {
            //        Item = EditTextField.Item,
            //        FieldName = EditTextField.FieldName,
            //        DefaultText = Text
            //    };
            //    tag.InnerHtml = renderer.Render(null, null);
            //}
            //else
            //{
            if (renderMode == TagRenderMode.Normal)
            {
                tag.SetInnerText(Text.IfNotNullOrEmpty() ?? Url);
            }

            if (IsEditMode)
            {
                tag.AddCssClass("scEnabledChrome");
                tag.Attributes.Add("sc-part-of", "field");
            }
            //}

            if (Title.IsNotNullOrEmpty())
            {
                tag.Attributes.Add("title", Title);
            }

            if (Class.IsNotNullOrEmpty())
            {
                tag.AddCssClass(Class);
            }

            tag.MergeAttributes(Attributes);

            return tag.ToString(renderMode);
        }
    }
}
