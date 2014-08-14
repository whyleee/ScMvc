using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var modelType = args.Result.GetType();
            var dataSource = GetDataSourceType(modelType);

            if (dataSource == RenderingDataSource.RenderingItem && !string.IsNullOrEmpty(args.Rendering.DataSource))
            {
                item = Sitecore.Context.Database.GetItem(args.Rendering.DataSource);
            }
            else if (dataSource == RenderingDataSource.ContextItem)
            {
                item = Sitecore.Context.Item;
            }

            if (item == null)
            {
                return;
            }
            
            var model = new SitecoreItemToEditableModelMapper().Map(item, modelType);

            _modelProcessor.Process(model);

            args.Result = model;
        }

        private RenderingDataSource GetDataSourceType(Type modelType)
        {
            var dataSource = RenderingDataSource.RenderingItem;
            var dataSourceAttribute = modelType.GetCustomAttribute<RenderingDataSourceAttribute>();

            if (dataSourceAttribute != null)
            {
                dataSource = dataSourceAttribute.DataSource;
            }

            return dataSource;
        }
    }
}
