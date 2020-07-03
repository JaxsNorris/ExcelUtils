using System;
using System.Collections.Generic;

namespace Common.Interfaces.Parsers
{
    public interface IParserService
    {
        DateTime? ParseDateTime(object? objValue, string[]? formats);
        double? ParseDouble(object? objValue);
        bool? ParseBoolean(object? objValue);
        T? ParseTextToLookupValue<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct;
        object? GetValue(object? inputValue, Type expectedType);
    }
}
