using Common.Exceptions;
using Common.Interfaces.Parsers;
using Common.Utils;
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

        public DateTime? Parse(object? objValue, string[]? formats)
        {
            if (objValue == null)
                return null;
            else if (objValue is DateTime dateTimeValue)
                return dateTimeValue;
            else if (objValue is string stringValue)
                return ParseFromString(stringValue, formats);
            else if (objValue.IsNumeric())
                return ParseFromDouble((double)objValue);

            throw UnsupportedDataTypeParserException.Create(objValue, ExpectedType);
        }

        private DateTime ParseFromString(string stringValue, string[]? formats)
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

            if (TryParseDouble(stringValue, out var doubleValue) && doubleValue.HasValue)
            {
                return ParseFromDouble(doubleValue.Value);
            }
            else
            {
                string? additionalUiMessage = null;
                if (formats != null && formats.Length != 0)
                {
                    additionalUiMessage = $"with the format(s) [{string.Join(',', formats)}]";
                }
                throw ParserException.CreateWithEnrichedMessage(stringValue, ExpectedType, additionalUiMessage, "Failed to parse string value to either DateTime or Double value");
            }
        }

        private bool TryParseDouble(string stringValue, out double? doubleOutputValue)
        {
            try
            {
                doubleOutputValue = _doubleParser.Parse(stringValue);
                return true;
            }
            catch (ParserException)
            {
                doubleOutputValue = null;
                return false;
            }
        }

        private static DateTime ParseFromDouble(double doubleValue)
        {
            try
            {
                return DateTime.FromOADate(doubleValue);
            }
            catch (ArgumentException ex)
            {
                throw ParserException.CreateFromException(doubleValue, ExpectedType, ex);
            }
        }
    }
}
