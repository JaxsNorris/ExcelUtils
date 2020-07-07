using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Utils
{
    public static class ConvertUtil
    {
        private const string NumericRegex = @"^-?[0-9][0-9,\.]*$";
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(double),
            typeof(decimal),
            typeof(float)
        };

        public static bool IsNumeric(this object value)
        {
            if (value == null)
                return false;
            return value.GetType().IsNumeric();
        }

        public static bool IsNumeric(this Type type)
        {
            if (type.IsEnum)
                return false;

            if (NumericTypes.Contains(type))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
                return false;

            return NumericTypes.Contains(underlyingType);
        }

        public static bool IsNumericString(this object value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value.ToString(), NumericRegex);
            }
            return false;
        }
    }
}
