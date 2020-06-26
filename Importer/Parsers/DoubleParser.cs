using Common.Exceptions;
using Common.Interfaces.Parsers;
using System;
using System.Globalization;

namespace Importer.Parsers
{
    internal class DoubleParser : IDoubleParser
    {
        private const string ExpectedType = "double";

        public double? Parse(string address, object? objValue)
        {
            if (objValue == null)
            {
                return null;
            }

            try
            {
                switch (objValue)
                {
                    case double doubleValue:
                        return doubleValue;
                    case short shortValue:
                        return shortValue;
                    case int intValue:
                        return intValue;
                    case long longValue:
                        return longValue;
                    case float floatValue:
                        return floatValue;
                    case decimal decimalValue:
                        return Convert.ToDouble(decimalValue);
                    case string stringValue:
                        return Convert.ToDouble(stringValue);
                    default:
                        throw ParserException.CreateUnsupportedDataTypeException(address, objValue, ExpectedType);
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
            {
                throw ParserException.CreateFromException(address, objValue, ExpectedType, ex);
            }
        }

        public double? Parse(string address, string stringValue, CultureInfo cultureInfo)
        {
            try
            {
                return double.Parse(stringValue, cultureInfo.NumberFormat);
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
            {
                throw ParserException.CreateFromException(address, stringValue, ExpectedType, ex);
            }
        }
    }
}
