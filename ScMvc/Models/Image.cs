using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ScMvc.Models
{
    public class Image : IHtmlString, ICustomizableModel, IEditableModel, ITemplatedModel, ICloneable
    {
        private IDictionary<string, object> _attributes;

        public Image()
        {
            _attributes = new Dictionary<string, object>();
            Crop = true;
        }

        private Image(IDictionary<string, object> attributes)
        {
            _attributes = attributes;
            Crop = true;
        }

        public string Src { get; set; }

        public string Alt { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int OrigWidth { get; set; }

        public int OrigHeight { get; set; }

        public bool Crop { get; set; }

        public bool Responsive { get; set; }

        public string Class { get; set; }

        public string FallbackSrc { get; set; }

        public IDictionary<string, object> Attributes
        {
            get { return _attributes; }
            set { _attributes = this.ParseAttributes(value); }
        }

        public bool IsEditMode { get; set; }

        public string Template { get; set; }

        public object Clone()
        {
            return new Image(_attributes)
            {
                Src = Src,
                Alt = Alt,
                Width = Width,
                Height = Height,
                OrigWidth = OrigWidth,
                OrigHeight = OrigHeight,
                Crop = Crop,
                Responsive = Responsive,
                Class = Class,
                FallbackSrc = FallbackSrc,
                IsEditMode = IsEditMode,
                Template = Template
            };
        }

        public string ToHtmlString()
        {
            return ToHtml(addMediaQueryToUrl: true, renderOrigSizes: true);
        }

        public override string ToString()
        {
            return ToHtml(addMediaQueryToUrl: false, renderOrigSizes: false);
        }

        public string ToHtml(bool addMediaQueryToUrl, bool renderOrigSizes)
        {
            if (string.IsNullOrEmpty(Src) && !string.IsNullOrEmpty(FallbackSrc))
            {
                Src = FallbackSrc;
            }
            if (IsEditMode && string.IsNullOrEmpty(Src))
            {
                Src = "/sitecore/shell/Themes/Standard/Images/WebEdit/default_image.png";
            }
            if (string.IsNullOrEmpty(Src))
            {
                return null;
            }

            var img = new TagBuilder("img");
            img.Attributes.Add("src", Src ?? "");

            if (!string.IsNullOrEmpty(Alt))
            {
                img.Attributes.Add("alt", Alt);
            }

            if (Width > 0 && Height > 0)
            {
                if (!Responsive)
                {
                    img.Attributes.Add("width", Width.ToString());
                    img.Attributes.Add("height", Height.ToString());
                }

                if (addMediaQueryToUrl)
                {
                    img.Attributes["src"] = GetTransformedImageUrl();
                }
            }
            else if (OrigWidth > 0 && OrigHeight > 0)
            {
                var widthAttr = renderOrigSizes ? "width" : "data-orig-width";
                var heightAttr = renderOrigSizes ? "height" : "data-orig-height";

                if (!Responsive || !renderOrigSizes)
                {
                    img.Attributes.Add(widthAttr, OrigWidth.ToString());
                    img.Attributes.Add(heightAttr, OrigHeight.ToString());
                }
            }

            // TODO: vspace/hspace are not supported (what if we use cropX and cropY here for editors)?

            if (!string.IsNullOrEmpty(Class))
            {
                img.AddCssClass(Class);
            }

            if (IsEditMode)
            {
                img.AddCssClass("scEnabledChrome");
                img.Attributes.Add("sc-part-of", "field");
            }

            img.MergeAttributes(Attributes);

            return img.ToString(TagRenderMode.SelfClosing);
        }

        public string GetTransformedImageUrl()
        {
            var url = Src;

            if (Width > 0 && Height > 0)
            {
                url = url + string.Format("{0}w={1}&h={2}", url.Contains('?') ? ':' : '?', Width, Height);

                if (Crop)
                {
                    url = url + "&useCustomFunctions=1&centerCrop=1";
                }
            }

            return url ?? "";
        }
    }
}
