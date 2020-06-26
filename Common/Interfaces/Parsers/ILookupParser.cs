using System.Collections.Generic;

namespace Common.Interfaces.Parsers
{
    public interface ILookupParser
    {
        T? Parse<T>(string value, IReadOnlyDictionary<T, string[]> dictionary, T? defaultValueOnNull, T? defaultValueOnFailedLookup) where T : struct;
        string? Parse(string value, IReadOnlyDictionary<string, string[]> dictionary, string? defaultValueOnNull, string? defaultValueOnFailedLookup);
    }
}
