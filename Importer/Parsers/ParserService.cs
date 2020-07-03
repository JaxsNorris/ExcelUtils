using Common.Exceptions;
using Common.Interfaces.Parsers;
using System;
using System.Collections.Generic;

namespace Importer.Parsers
{
    internal class ParserService : IParserService
    {
        private readonly IDateTimeParser _dateTimeParser;
        private readonly IDoubleParser _doubleParser;
        private readonly ILookupParser _lookupParser;
        private readonly IBooleanParser _booleanParser;

        public ParserService(IDateTimeParser dateTimeParser, IDoubleParser doubleParser, ILookupParser lookupParser, IBooleanParser booleanParser)
        {
            _dateTimeParser = dateTimeParser;
            _doubleParser = doubleParser;
            _lookupParser = lookupParser;
            _booleanParser = booleanParser;
        }

        public DateTime? ParseDateTime(object? objValue, string[]? formats)
        {
            return _dateTimeParser.Parse(objValue, formats);
        }

        public double? ParseDouble(object? objValue)
        {
            return _doubleParser.Parse(objValue);
        }

        public T? ParseTextToLookupValue<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct
        {
            return _lookupParser.Parse(value, dictionary, defaultValueOnNull, defaultValueOnFailedLookup);
        }

        public object? GetValue(object? inputValue, Type expectedType)
        {
            if (inputValue == null)
            {
                return null;
            }
            else if (inputValue.GetType() == expectedType)
            {
                return inputValue;
            }
            else if (expectedType == typeof(string))
            {
                return inputValue.ToString();
            }
            else if (expectedType == typeof(double) || expectedType == typeof(double?))
            {
                return ParseDouble(inputValue);
            }
            else if (expectedType == typeof(DateTime) || expectedType == typeof(DateTime?))
            {
                return ParseDateTime(inputValue, null);
            }
            else if (expectedType == typeof(bool) || expectedType == typeof(bool?))
            {
                if (inputValue is bool?)
                {
                    return inputValue;
                }
                return ParseBoolean(inputValue);
            }
            throw UnsupportedDataTypeParserException.Create(inputValue, expectedType.Name);
        }

        public bool? ParseBoolean(object? objValue)
        {
            return _booleanParser.Parse(objValue);
        }
    }
}
