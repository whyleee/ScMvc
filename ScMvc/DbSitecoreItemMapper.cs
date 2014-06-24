using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perks;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ScMvc
{
    public class DbSitecoreItemMapper
    {
        public virtual T Map<T>(Item item) where T : IEditableModel
        {
            var to = typeof (T);
            var target = (T) Activator.CreateInstance(to);

            target.Item = item;
            target.IsEditMode = Sitecore.Context.PageMode.IsPageEditorEditing;

            if (item == null)
            {
                return target;
            }

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
                    value = fieldValue;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    value = fieldValue == "1";
                }
                else if (property.PropertyType == typeof(int))
                {
                    value = fieldValue.IsNullOrEmpty() ? 0 : int.Parse(fieldValue, CultureInfo.InvariantCulture);
                }
                else if (property.PropertyType == typeof(double))
                {
                    value = fieldValue.IsNullOrEmpty() ? 0d : double.Parse(fieldValue, CultureInfo.InvariantCulture);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    value = ((DateField)field).DateTime;
                }
                //else if (property.PropertyType == typeof(Image))
                //{
                //    value = new ImageFieldModelMapper().ToModel(field);
                //}
                //else if (property.PropertyType == typeof(Link))
                //{
                //    value = new LinkFieldModelMapper().ToModel(field);
                //}
                else if (property.PropertyType.Is(typeof(IEnumerable<ID>)))
                {
                    value = GetIds(fieldValue);
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
