using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScMvc.Aids;
using ScMvc.Models.Mappers;
using Sitecore.Data.Items;

namespace ScMvc.Models
{
    public static class ModelExtensions
    {
        public static IDictionary<string, object> ParseAttributes(this ICustomizableModel model, IDictionary<string, object> attributes)
        {
            var otherAttributes = new Dictionary<string, object>();

            foreach (var attr in attributes)
            {
                var prop = model.GetType().GetProperties().FirstOrDefault(p => string.Equals(p.Name, attr.Key, StringComparison.InvariantCultureIgnoreCase));

                if (prop != null)
                {
                    if (attr.Key.ToLower() == "class" && !string.IsNullOrEmpty(model.Class))
                    {
                        model.Class += " " + attr.Value;
                    }
                    else
                    {
                        var value = attr.Value;

                        if (prop.PropertyType.Is<int>() && !(attr.Value is int))
                        {
                            value = Convert.ToInt32(attr.Value);
                        }
                        else if (prop.PropertyType.Is<double>() && !(attr.Value is double))
                        {
                            value = Convert.ToDouble(attr.Value);
                        }
                        else if (prop.PropertyType.Is<bool>() && !(attr.Value is bool))
                        {
                            value = Convert.ToBoolean(attr.Value);
                        }

                        prop.SetValue(model, value, null);
                    }
                }
                else
                {
                    otherAttributes.Add(attr.Key, attr.Value);
                }
            }

            return otherAttributes;
        }

        public static T MapFrom<T>(this T model, Item item, SitecoreItemToEditableModelMapper mapper) where T : IEditableItemModel
        {
            return mapper.Map(item, model);
        }
    }
}
