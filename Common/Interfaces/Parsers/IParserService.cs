using System;
using System.Collections.Generic;

namespace Common.Interfaces.Parsers
{
    public interface IParserService
    {
        DateTime? ParseDateTime(string fullAddress, object? objValue, string[]? formats);
        double? ParseDouble(string fullAddress, object? objValue);
        T? ParseTextToLookupValue<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct;
        object? GetValue(string fullAddress, object? inputValue, Type expectedType);
    }
}
