using System;
using System.Collections.Generic;
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
    }
}
