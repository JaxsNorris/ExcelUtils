using Common.Exceptions;
using Common.Interfaces.Parsers;
using System;
using System.Globalization;

namespace Importer.Parsers
{
    internal class DateTimeParser : IDateTimeParser
    {
        private const string ExpectedType = "date";
        private readonly IDoubleParser _doubleParser;

        public DateTimeParser(IDoubleParser doubleParser)
        {
            _doubleParser = doubleParser;
        }

        public DateTime? Parse(string address, object? objValue, string[]? formats)
        {
            switch (objValue)
            {
                case null:
                    return null;
                case DateTime dateTimeValue:
                    return dateTimeValue;
                case double doubleValue:
                    return ParseFromDouble(address, doubleValue);
                case string stringValue:
                    return ParseFromString(address, stringValue, formats);
                default:
                    throw ParserException.CreateUnsupportedDataTypeException(address, objValue, ExpectedType);
            }
        }

        private DateTime ParseFromString(string address, string stringValue, string[]? formats)
        {
            if (formats != null && formats.Length != 0
                && DateTime.TryParseExact(stringValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
            else if (DateTime.TryParse(stringValue, out parsedDate))
            {
                return parsedDate;
            }

            if (TryParseDouble(address, stringValue, out var doubleValue) && doubleValue.HasValue)
            {
                return ParseFromDouble(address, doubleValue.Value);
            }
            else
            {
                string? additionalUiMessage = null;
                if (formats != null && formats.Length != 0)
                {
                    additionalUiMessage = $"with the format(s) [{string.Join(',', formats)}]";
                }
                throw ParserException.CreateWithEnrichedMessage(address, stringValue, ExpectedType, additionalUiMessage, "Failed to parse string value to either DateTime or Double value");
            }
        }

        private bool TryParseDouble(string address, string stringValue, out double? doubleValue)
        {
            try
            {
                doubleValue = _doubleParser.Parse(address, stringValue);
                return true;
            }
            catch (ParserException)
            {
                doubleValue = null;
                return false;
            }
        }

        private static DateTime ParseFromDouble(string address, double doubleValue)
        {
            try
            {
                return DateTime.FromOADate(doubleValue);
            }
            catch (ArgumentException ex)
            {
                throw ParserException.CreateFromException(address, doubleValue, ExpectedType, ex);
            }
        }
    }
}
