using System;
using System.Collections.Generic;
using System.Linq;
using Perks;
using ScMvc.Models;
using ScMvc.Models.Mappers;
using ScMvc.Models.Processors;
using ScMvc.Rendering;
using Sitecore.Data.Items;
using Sitecore.Mvc.Pipelines.Response.GetModel;

namespace ScMvc.Pipelines
{
    public class InitializeEditableItemModelFromDb : GetModelProcessor
    {
        private readonly PropertyModelProcessor _modelProcessor = new SitecorePageEditorPropertyModelProcessor();

        public override void Process(GetModelArgs args)
        {
            if (args.Result == null)
            {
                return;
            }
            if (!(args.Result is IEditableItemModel))
            {
                return;
            }

            Item item = null;

            if (args.Rendering.DataSource.IsNotNullOrEmpty())
            {
                item = Sitecore.Context.Database.GetItem(args.Rendering.DataSource);
            }

            if (item == null)
            {
                return;
            }

            var modelType = args.Result.GetType();
            var model = new DbSitecoreItemMapper().Map(item, modelType);

            _modelProcessor.Process(model);

            args.Result = model;
        }
    }
}
