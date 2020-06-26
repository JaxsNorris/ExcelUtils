using System;

namespace Common.Interfaces.Parsers
{
    public interface IDateTimeParser
    {
        DateTime? Parse(string fullAddress, object? objValue, string[]? formats);
    }
}
