using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ScMvc.Rendering
{
    public class FillEmptyModelFieldsInPageEditorActionFilter : ActionFilterAttribute
    {
        private readonly FillEmptyFieldsInPageEditorModelProcessor.Settings _settings;

        public FillEmptyModelFieldsInPageEditorActionFilter(FillEmptyFieldsInPageEditorModelProcessor.Settings settings = null)
        {
            _settings = settings;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var processor = new FillEmptyFieldsInPageEditorModelProcessor(_settings);
            processor.Process(filterContext);
        }
    }
}
