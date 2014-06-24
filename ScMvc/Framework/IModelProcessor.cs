﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ScMvc.Framework
{
    public interface IModelProcessor
    {
        void Process(ResultExecutingContext context);
    }
}
