namespace Common.Interfaces.Parsers
{
    public interface IDoubleParser
    {
        double? Parse(string fullAddress, object? objValue);
    }
}
