using Common.Exceptions;
using Common.Interfaces.Parsers;
using Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Importer.Parsers
{
    public class EnumParser : IEnumParser
    {
        private readonly ConcurrentDictionary<string, IReadOnlyDictionary<object, string[]>> _enumLookupDictionary = new ConcurrentDictionary<string, IReadOnlyDictionary<object, string[]>>();

        public T? Parse<T>(object? objValue) where T : struct
        {
            return (T?)Parse(objValue, typeof(T));
        }

        public object? Parse(object? objValue, Type expectedType)
        {
            if (objValue == null)
                return null;

            var objValueType = objValue.GetType();
            if (objValueType.IsEnum && objValueType == expectedType)
            {
                return objValue;
            }
            else if (objValue is string stringValue)
            {
                if (Enum.TryParse(expectedType, stringValue, true, out var parsedEnum))
                    return parsedEnum;

                var lookup = _enumLookupDictionary.GetOrAdd(GetKey(expectedType), (key) => expectedType.CreateEnumLookupDictionary());
                return LookupEnumValue(expectedType, stringValue, lookup);
            }

            throw UnsupportedDataTypeParserException.Create(objValue, "string");
        }

        private object? LookupEnumValue(Type expectedType, string value, IReadOnlyDictionary<object, string[]> dictionary)
        {
            var trimmedValue = value.Trim();
            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Value.Contains(trimmedValue, StringComparer.InvariantCultureIgnoreCase))
                    return keyValuePair.Key;
            }

            throw ParserException.CreateFailedLookup(value, expectedType);
        }

        private string GetKey(Type type)
        {
            return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
        }

    }
}
