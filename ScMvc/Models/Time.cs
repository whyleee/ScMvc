using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ScMvc.Aids;

namespace ScMvc.Models
{
    public class Time : IHtmlString, ICustomizableModel, IEditableModel, ITemplatedModel, IRenderToTag, ICloneable
    {
        private IDictionary<string, object> _attributes;

        public Time(DateTime value)
        {
            Value = value;
            _attributes = new Dictionary<string, object>();
        }

        private Time(IDictionary<string, object> attributes)
        {
            Ensure.ArgumentNotNull(attributes, "attributes");

            _attributes = attributes;
        }

        public DateTime Value { get; set; }

        public string Format { get; set; }

        public string Class { get; set; }

        public IDictionary<string, object> Attributes
        {
            get { return _attributes; }
            set { _attributes = this.ParseAttributes(value); }
        }

        public bool IsEditMode { get; set; }

        public string Template { get; set; }

        public object Clone()
        {
            return new Time(_attributes)
            {
                Value = Value,
                Format = Format,
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
            return new HtmlString(ToHtml(TagRenderMode.StartTag));
        }

        public IHtmlString EndTag()
        {
            return new HtmlString(ToHtml(TagRenderMode.EndTag));
        }

        public string ToHtml(TagRenderMode renderMode)
        {
            var tag = new TagBuilder("time");

            if (renderMode == TagRenderMode.EndTag)
            {
                return tag.ToString(renderMode);
            }

            tag.Attributes.Add("datetime", Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(Class))
            {
                tag.AddCssClass(Class);
            }

            if (IsEditMode)
            {
                tag.AddCssClass("scEnabledChrome");
                tag.Attributes.Add("sc-part-of", "field");
            }

            tag.InnerHtml = Value.ToString(Format);

            tag.MergeAttributes(Attributes);

            return tag.ToString(renderMode);
        }
    }
}
