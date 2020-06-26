using Common.Interfaces.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Importer.Parsers
{
    internal class LookupParser : ILookupParser
    {
        public T? Parse<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValueOnNull;
            }

            var trimmedValue = value.Trim();
            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Value.Contains(trimmedValue, StringComparer.InvariantCultureIgnoreCase))
                    return keyValuePair.Key;
            }

            return defaultValueOnFailedLookup;
        }

        public string? Parse(string value, IReadOnlyDictionary<string, string[]> dictionary, string? defaultValueOnNull, string? defaultValueOnFailedLookup)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValueOnNull;
            }

            var trimmedValue = value.Trim();
            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Value.Contains(trimmedValue, StringComparer.InvariantCultureIgnoreCase))
                    return keyValuePair.Key;
            }

            return defaultValueOnFailedLookup;
        }
    }
}
