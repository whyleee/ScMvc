﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ScMvc.Aids;
using Sitecore.Configuration;
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

        public virtual T Map<T>(Item item, T model) where T : IEditableItemModel
        {
            Ensure.ArgumentNotNull(model, "model");

            return (T) Map(item, model.GetType(), model);
        }

        public virtual IEditableItemModel Map(Item item, Type to)
        {
            return Map(item, to, model: null);
        }

        public virtual T MapField<T>(Item item, string fieldName)
        {
            return (T) MapField(item.Fields[fieldName], typeof (T));
        }

        private IEditableItemModel Map(Item item, Type to, IEditableItemModel model)
        {
            Ensure.ArgumentNotNull(to, "to");

            if (!to.Is(typeof (IEditableItemModel)))
            {
                throw new ArgumentException(string.Format("Target type should implement '{0}' interface", to.FullName), "to");
            }

            var target = model ?? (IEditableItemModel) Activator.CreateInstance(to);
            var isEditMode = Sitecore.Context.PageMode.IsPageEditorEditing;

            if (item == null)
            {
                return target;
            }

            target.Item = item;
            target.IsEditMode = isEditMode;

            foreach (var property in to.GetProperties())
            {
                if (property.GetCustomAttribute<IgnoreMapAttribute>() != null)
                {
                    continue;
                }

                if (model != null && property.GetValue(model) != null)
                {
                    continue;
                }

                if (property.Name == "Name")
                {
                    property.SetValue(target, item.Name);
                }

                if (property.Name == "DisplayName")
                {
                    property.SetValue(target, item.DisplayName);
                    continue;
                }

                if (property.Name == "Url")
                {
                    property.SetValue(target, item.GetFriendlyUrl());
                    continue;
                }

                var field = item.Fields[property.Name];

                if (field == null)
                {
                    // TODO: probably this is too slow to call on every model field
                    // consided hardcoded appoach
                    field = TryGetInternalField(item, property.Name);

                    if (field == null)
                    {
                        continue;
                    }
                }

                var value = MapField(field, property.PropertyType);
                property.SetValue(target, value);
            }

            return target;
        }

        private object MapField(Field field, Type to)
        {
            var fieldValue = field.Value;
            object value = fieldValue;

            if (to == typeof(string))
            {
                if (field.TypeKey == "rich text" && !string.IsNullOrEmpty(fieldValue))
                {
                    value = LinkManager.ExpandDynamicLinks(fieldValue, Settings.Rendering.SiteResolving);
                }
                else
                {
                    value = fieldValue;
                }
            }
            else if (to == typeof(bool))
            {
                value = fieldValue == "1";
            }
            else if (to == typeof(int))
            {
                value = string.IsNullOrEmpty(fieldValue) ? 0 : int.Parse(fieldValue, CultureInfo.InvariantCulture);
            }
            else if (to == typeof(double))
            {
                value = string.IsNullOrEmpty(fieldValue) ? 0d : double.Parse(fieldValue, CultureInfo.InvariantCulture);
            }
            else if (to == typeof(DateTime))
            {
                value = ((DateField)field).DateTime;
            }
            else if (to == typeof(Image))
            {
                value = new ImageFieldToModelMapper().ToModel(field);
            }
            else if (to == typeof(Link))
            {
                value = new LinkFieldToModelMapper().ToModel(field);
            }
            else if (to.Is(typeof(IEnumerable<ID>)))
            {
                value = GetIds(fieldValue);
            }
            else if (to.Is<IEditableItemModel>() && (field.TypeKey == "droplink" || field.TypeKey == "droptree"))
            {
                if (string.IsNullOrEmpty(fieldValue))
                {
                    value = null;
                }
                else
                {
                    var linkedItem = Sitecore.Context.Database.GetItem(fieldValue);
                    value = Map(linkedItem, to);
                }
            }

            return value;
        }

        private IList<ID> GetIds(string value)
        {
            return value.Split('|').Where(ID.IsID).Select(ID.Parse).ToList();
        }

        internal static Field TryGetInternalField(Item item, string mappedFieldName)
        {
            var internalFieldName = mappedFieldName;

            if (TryFieldRenames(item, ref internalFieldName,
                fn => "__" + fn.ToFriendlyString(),
                fn => "__" + Regex.Replace(fn.ToFriendlyString(), @"( [A-Z])", match => match.Value.ToLower())
                ))
            {
                return item.Fields[internalFieldName];
            }

            return null;
        }

        private static bool TryFieldRenames(Item item, ref string fieldName, params Func<string, string>[] renames)
        {
            foreach (var rename in renames)
            {
                var renamedFieldName = rename(fieldName);

                if (item.Fields[renamedFieldName] != null)
                {
                    fieldName = renamedFieldName;
                    return true;
                }
            }

            return false;
        }
    }
}
