﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perks;

namespace ScMvc.Models
{
    public static class ModelHtmlExtensions
    {
        public static IDictionary<string, object> ParseAttributes(this object model, IDictionary<string, object> attributes)
        {
            var otherAttributes = new Dictionary<string, object>();

            foreach (var attr in attributes)
            {
                var prop = model.GetType().GetProperties().FirstOrDefault(p => string.Equals(p.Name, attr.Key, StringComparison.InvariantCultureIgnoreCase));

                if (prop != null)
                {
                    if (model is ICustomizableModel && attr.Key.ToLower() == "class" && ((ICustomizableModel)model).Class.IsNotNullOrEmpty())
                    {
                        ((ICustomizableModel)model).Class += " " + attr.Value;
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
    }
}
