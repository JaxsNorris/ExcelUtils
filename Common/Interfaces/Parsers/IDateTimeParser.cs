using System;

namespace Common.Interfaces.Parsers
{
    public interface IDateTimeParser
    {
        DateTime? Parse(object? objValue, string[]? formats);
    }
}
