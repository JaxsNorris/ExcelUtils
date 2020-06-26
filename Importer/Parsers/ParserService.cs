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

        public ParserService(IDateTimeParser dateTimeParser, IDoubleParser doubleParser, ILookupParser lookupParser)
        {
            _dateTimeParser = dateTimeParser;
            _doubleParser = doubleParser;
            _lookupParser = lookupParser;
        }

        public DateTime? ParseDateTime(string fullAddress, object? objValue, string[]? formats)
        {
            return _dateTimeParser.Parse(fullAddress, objValue, formats);
        }

        public double? ParseDouble(string fullAddress, object? objValue)
        {
            return _doubleParser.Parse(fullAddress, objValue);
        }

        public T? ParseTextToLookupValue<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct
        {
            return _lookupParser.Parse(value, dictionary, defaultValueOnNull, defaultValueOnFailedLookup);
        }

        public object? GetValue(string fullAddress, object? inputValue, Type expectedType)
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
                return ParseDouble(fullAddress, inputValue);
            }
            else if (expectedType == typeof(DateTime) || expectedType == typeof(DateTime?))
            {
                return ParseDateTime(fullAddress, inputValue, null);
            }
            else if (expectedType == typeof(bool) || expectedType == typeof(bool?))
            {
                if (inputValue is bool?)
                {
                    return inputValue;
                }
                if (inputValue is string stringValue)
                {
                    return ParseTextToLookupValue(stringValue, ParserConstants.BoolDefaultMapping, null, null);
                }
                throw new NotSupportedException("Can only parse bool from string");
            }
            throw new NotSupportedException("Only string,bool, double or date time supported");
        }
    }
}
