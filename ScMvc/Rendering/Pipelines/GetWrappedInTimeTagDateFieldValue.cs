using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml.Xsl;

namespace ScMvc.Rendering.Pipelines
{
    public class GetWrappedInTimeTagDateFieldValue : GetDateFieldValue
    {
        protected override DateRenderer CreateRenderer()
        {
            return new TimeTagDateRenderer();
        }
    }

    public class TimeTagDateRenderer : DateRenderer
    {
        // TODO: make it configurable, whether I want to render the datetime in the <time> tag
        public override RenderFieldResult Render()
        {
            var result = base.Render();

            if (!string.IsNullOrEmpty(result.FirstPart))
            {
                var dateTime = DateUtil.IsoDateToDateTime(FieldValue, DateTime.MinValue);
                var validHtmlDateTime = dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mmzzz");

                result.FirstPart = string.Format("<time datetime=\"{0}\">{1}</time>",
                    validHtmlDateTime, result.FirstPart
                );
            }

            return result;
        }
    }
}
