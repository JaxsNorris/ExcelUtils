using System;

namespace Common.Interfaces.Parsers
{
    public interface IEnumParser
    {
        object? Parse(object? objValue, Type expectedType);
        T? Parse<T>(object? objValue) where T : struct;
    }
}
