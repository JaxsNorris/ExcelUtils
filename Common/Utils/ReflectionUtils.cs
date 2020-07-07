using Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Utils
{
    public static class ReflectionUtils
    {
        public static Dictionary<PropertyInfo, T> CreateLookupDictionary<T>(this Type type) where T : Attribute
        {
            Dictionary<PropertyInfo, T> propertyLookup = new Dictionary<PropertyInfo, T>();

            var props = type.GetProperties();
            foreach (var property in props)
            {
                var attribute = property.GetCustomAttribute<T>();
                if (attribute == null)
                    continue;
                propertyLookup.Add(property, attribute);
            }

            return propertyLookup;
        }

        public static IReadOnlyDictionary<T, string[]> CreateEnumLookupDictionary<T>() where T : struct
        {
            var dictionary = new Dictionary<T, string[]>();
            var fields = typeof(T).GetFields()
            .Where(field => !field.IsSpecialName);
            var enumValues = typeof(T).GetEnumValues();
            foreach (var enumValue in enumValues)
            {
                if (enumValue == null)
                    continue;

                var attribute = fields.SingleOrDefault(field => field.Name == enumValue.ToString())
                                    ?.GetCustomAttribute<EnumLookupAttribute>();
                if (attribute != null && enumValue is T)
                    dictionary.Add((T)enumValue, attribute.LookupDictionaryValues);
            }
            return dictionary;
        }

        public static IReadOnlyDictionary<object, string[]> CreateEnumLookupDictionary(this Type expectedType)
        {
            var dictionary = new Dictionary<object, string[]>();
            var fields = expectedType.GetFields()
                                .Where(field => !field.IsSpecialName);
            var enumValues = expectedType.GetEnumValues();

            foreach (var enumValue in enumValues)
            {
                if (enumValue == null)
                    continue;

                var attribute = fields.SingleOrDefault(field => field.Name == enumValue.ToString())
                                    ?.GetCustomAttribute<EnumLookupAttribute>();

                if (attribute != null)
                    dictionary.Add(enumValue, attribute.LookupDictionaryValues);
            }
            return dictionary;
        }
    }
}
