using System.IO;

namespace Common.Interfaces
{
    public interface IReaderFactory
    {
        IReader CreateReader(FileInfo file, string worksheetName);
    }
}
