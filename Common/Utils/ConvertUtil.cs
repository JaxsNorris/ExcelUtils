using System;
using System.Text.RegularExpressions;

namespace Common.Utils
{
    public static class ConvertUtil
    {
        private const string NumericRegex = @"^-?[0-9][0-9,\.]*$";

        public static bool IsNumeric(this object value)
        {
            if (value == null)
                return false;
            return value.GetType().IsNumeric();
        }

        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
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
