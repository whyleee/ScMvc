using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ScMvc.Framework;

namespace ScMvc
{
    public class SitecorePageEditorActionFilter : ActionFilterAttribute
    {
        private readonly SitecorePageEditorPropertyModelProcessor.Settings _settings;

        public SitecorePageEditorActionFilter(SitecorePageEditorPropertyModelProcessor.Settings settings = null)
        {
            _settings = settings;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var processor = new SitecorePageEditorPropertyModelProcessor(_settings);
            processor.Process(filterContext);
        }
    }
}
