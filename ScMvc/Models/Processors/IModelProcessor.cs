using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ScMvc.Models.Processors
{
    public interface IModelProcessor
    {
        void Process(ResultExecutingContext context);
    }
}
