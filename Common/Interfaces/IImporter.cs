using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IImporter<out T>
    {
        IEnumerable<T> Import(string fullpath, string worksheetName, int dataRowStart, int numberOfItems);
    }
}
