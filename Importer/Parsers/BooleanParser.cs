using Common.Exceptions;
using Common.Interfaces.Parsers;

namespace Importer.Parsers
{
    internal class BooleanParser : IBooleanParser
    {
        private const string ExpectedType = "boolean";
        private readonly ILookupParser _lookupParser;

        public BooleanParser(ILookupParser lookupParser)
        {
            _lookupParser = lookupParser;
        }

        public bool? Parse(object? objValue)
        {
            switch (objValue)
            {
                case null:
                    return null;
                case bool booleanValue:
                    return booleanValue;
                case double doubleValue:
                    return ParseFromDouble(doubleValue);
                case int intValue:
                    return ParseFromDouble(intValue);
                case long longValue:
                    return ParseFromDouble(longValue);
                case short shortValue:
                    return ParseFromDouble(shortValue);
                case float floatValue:
                    return ParseFromDouble(floatValue);
                case string stringValue:
                    return ParseFromString(stringValue);
                default:
                    throw UnsupportedDataTypeParserException.Create(objValue, ExpectedType);
            }
        }

        private bool? ParseFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var parsedBool = _lookupParser.Parse(value, ParserConstants.BoolDefaultMapping, null, null);
            if (parsedBool.HasValue)
                return parsedBool.Value;

            throw ParserException.CreateUnsupportedValueException(value, ExpectedType);
        }

        private bool? ParseFromDouble(double value)
        {
            if (value == 0)
                return false;
            else if (value == 1)
                return true;

            throw ParserException.CreateUnsupportedValueException(value, ExpectedType);
        }
    }
}
