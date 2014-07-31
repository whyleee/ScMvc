using System;
using System.Collections.Generic;
using System.Linq;
using ScMvc.Models;
using ScMvc.Models.Mappers;
using ScMvc.Models.Processors;
using ScMvc.Rendering;
using Sitecore.Data.Items;
using Sitecore.Mvc.Pipelines.Response.GetModel;

namespace ScMvc.Pipelines
{
    public class InitializeEditableModelFromItem : GetModelProcessor
    {
        private readonly PropertyModelProcessor _modelProcessor = new FillEmptyFieldsInPageEditorModelProcessor();

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

            if (!string.IsNullOrEmpty(args.Rendering.DataSource))
            {
                item = Sitecore.Context.Database.GetItem(args.Rendering.DataSource);
            }

            if (item == null)
            {
                return;
            }

            var modelType = args.Result.GetType();
            var model = new SitecoreItemToEditableModelMapper().Map(item, modelType);

            _modelProcessor.Process(model);

            args.Result = model;
        }
    }
}
