using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ScMvc.Aids;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace ScMvc.Models.Mappers
{
    public class SitecoreItemToEditableModelMapper
    {
        public virtual T Map<T>(Item item) where T : IEditableItemModel
        {
            return (T) Map(item, typeof (T));
        }

        public virtual IEditableItemModel Map(Item item, Type to)
        {
            Ensure.ArgumentNotNull(to, "to");

            if (!to.Is(typeof (IEditableItemModel)))
            {
                throw new ArgumentException(string.Format("Target type should implement '{0}' interface", to.FullName), "to");
            }

            var target = (IEditableItemModel) Activator.CreateInstance(to);
            var isEditMode = Sitecore.Context.PageMode.IsPageEditorEditing;

            if (item == null)
            {
                return target;
            }

            target.Item = item;
            target.IsEditMode = isEditMode;

            foreach (var property in to.GetProperties())
            {
                var field = item.Fields[property.Name];

                if (field == null)
                {
                    continue;
                }

                var fieldValue = field.Value;
                object value = fieldValue;

                if (property.PropertyType == typeof(string))
                {
                    if (field.TypeKey == "rich text" && !string.IsNullOrEmpty(fieldValue))
                    {
                        value = LinkManager.ExpandDynamicLinks(fieldValue);
                    }
                    else
                    {
                        value = fieldValue;
                    }
                }
                else if (property.PropertyType == typeof(bool))
                {
                    value = fieldValue == "1";
                }
                else if (property.PropertyType == typeof(int))
                {
                    value = string.IsNullOrEmpty(fieldValue) ? 0 : int.Parse(fieldValue, CultureInfo.InvariantCulture);
                }
                else if (property.PropertyType == typeof(double))
                {
                    value = string.IsNullOrEmpty(fieldValue) ? 0d : double.Parse(fieldValue, CultureInfo.InvariantCulture);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    value = ((DateField)field).DateTime;
                }
                else if (property.PropertyType == typeof(Image))
                {
                    value = new ImageFieldToModelMapper().ToModel(field);
                }
                else if (property.PropertyType == typeof(Link))
                {
                    value = new LinkFieldToModelMapper().ToModel(field);
                }
                else if (property.PropertyType.Is(typeof(IEnumerable<ID>)))
                {
                    value = GetIds(fieldValue);
                }
                else if (property.PropertyType.Is<IEditableItemModel>()
                    && !string.IsNullOrEmpty(fieldValue)
                    && (field.TypeKey == "droplink" || field.TypeKey == "droptree"))
                {
                    var linkedItem = Sitecore.Context.Database.GetItem(fieldValue);
                    value = Map(linkedItem, property.PropertyType);
                }

                property.SetValue(target, value);
            }

            return target;
        }

        private IList<ID> GetIds(string value)
        {
            return value.Split('|').Where(ID.IsID).Select(ID.Parse).ToList();
        }
    }
}
