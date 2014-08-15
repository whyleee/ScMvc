﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            args.Result.FirstPart = new SelectControl().Render(args);
        }
    }
}
