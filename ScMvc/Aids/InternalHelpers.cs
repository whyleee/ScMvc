using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScMvc.Aids
{
    // Copy-pasted extensions from "Perks" project
    internal static class InternalHelpers
    {
        public static TOut IfNotNull<TIn, TOut>(this TIn source, Func<TIn, TOut> result) where TIn : class
        {
            return source != null ? result(source) : default(TOut);
        }

        public static string IfNotNullOrEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source) ? source : null;
        }

        public static bool Is<T>(this Type type)
        {
            return type.Is(typeof(T));
        }

        public static bool Is(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType))
                {
                    return true;
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }

                return type.BaseType != null && Is(type.BaseType, baseType);
            }

            return baseType.IsAssignableFrom(type);
        }

        public static Type GetCollectionItemType(this Type type)
        {
            if (!type.Is<IEnumerable>())
            {
                return null;
            }

            return type.GetElementType() ?? type.GetGenericArguments().FirstOrDefault();
        }

        public static string ToFriendlyString(this string text)
        {
            return Regex.Replace(text, @"([a-z](?=[A-Z]|\d)|[A-Z](?=[A-Z][a-z]|\d)|\d(?=[A-Z][a-z]))", "$1 ");
        }
    }
}
