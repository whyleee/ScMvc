using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Perks;

namespace ScMvc.Framework
{
    public abstract class PropertyModelProcessor : IModelProcessor
    {
        protected ResultExecutingContext Context { get; set; }

        public virtual void Process(ResultExecutingContext context)
        {
            if (!(context.Result is ViewResultBase))
            {
                return;
            }

            if (!IsApplicableFor(context))
            {
                return;
            }

            Context = context;
            var model = ((ViewResultBase)context.Result).Model;

            if (model == null)
            {
                return;
            }

            Prepare(model);
            ProcessModel(model);
        }

        protected virtual bool IsApplicableFor(ResultExecutingContext context)
        {
            return true;
        }

        protected virtual void Prepare(object model) { }

        protected virtual bool ProcessModel(object model)
        {
            if (model == null)
            {
                return false;
            }

            if (model is IEnumerable)
            {
                var itemType = model.GetType().GetCollectionItemType();

                if (itemType != null && CanHandle(itemType))
                {
                    foreach (var item in (IEnumerable)model)
                    {
                        ProcessModelProperties(item);
                    }

                    return true;
                }
            }
            else
            {
                if (CanHandle(model.GetType()))
                {
                    ProcessModelProperties(model);
                    return true;
                }
            }

            return false;
        }

        private void ProcessModelProperties(object model)
        {
            foreach (var property in model.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(model, null);

                if (!ProcessModel(propertyValue))
                {
                    ProcessModelProperty(model, property, propertyValue);
                }
            }
        }

        protected abstract bool CanHandle(Type modelType);

        protected abstract void ProcessModelProperty(object model, PropertyInfo property, object value);
    }
}
