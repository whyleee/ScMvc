﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScMvc.Aids;
using ScMvc.Models;
using ScMvc.Models.Processors;

namespace ScMvc.Rendering
{
    public class FillEmptyFieldsInPageEditorModelProcessor : PropertyModelProcessor
    {
        private readonly Settings _settings;

        public FillEmptyFieldsInPageEditorModelProcessor(Settings settings = null)
        {
            _settings = settings ?? new Settings();
        }

        public class Settings
        {
            public Settings()
            {
                ShouldBeHandled = mt => mt.Is<IEditableItemModel>();
            }

            public Func<Type, bool> ShouldBeHandled { get; set; }
        }

        protected override bool CanHandle(Type modelType)
        {
            var isEditMode = Sitecore.Context.PageMode.IsPageEditorEditing;

            return isEditMode && _settings.ShouldBeHandled(modelType);
        }

        protected override void ProcessModelProperty(object model, PropertyInfo property, object value)
        {
            if (property.GetCustomAttribute<IgnoreEditAttribute>() != null)
            {
                return;
            }

            if (property.PropertyType == typeof (string) && string.IsNullOrEmpty((string) value))
            {
                property.SetValue(model, "[Empty]");
            }
            else if (property.PropertyType == typeof (int) && (int) value == 0)
            {
                property.SetValue(model, 1);
            }
            else if (property.PropertyType == typeof (double) && (double) value == 0)
            {
                property.SetValue(model, 1d);
            }
            else if (property.PropertyType == typeof (DateTime) && (DateTime) value == DateTime.MinValue)
            {
                property.SetValue(model, DateTime.MaxValue);
            }
            else if (property.PropertyType == typeof (Image) && value == null)
            {
                property.SetValue(model, new Image());
            }
            else if (property.PropertyType == typeof (Link) && value == null)
            {
                property.SetValue(model, new Link());
            }
            else if (property.PropertyType.Is<IEnumerable>() && (value == null || !((IEnumerable) value).Cast<object>().Any()))
            {
                var generic = property.PropertyType.Is(typeof (IEnumerable<>));
                var itemType = property.PropertyType.GetCollectionItemType();
                var item = generic ? Activator.CreateInstance(itemType) : new object();

                var collection = Activator.CreateInstance(typeof (List<>).MakeGenericType(itemType));
                collection.GetType().GetMethod("Add").Invoke(collection, new []{item});
                    
                property.SetValue(model, collection);
            }
            else if (property.PropertyType.Is<IEditableItemModel>())
            {
                property.SetValue(model, Activator.CreateInstance(property.PropertyType));
            }
        }
    }
}
