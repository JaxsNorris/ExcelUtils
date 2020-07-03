using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IDataGenerator
    {
        Task GenerateData(string fullpath, string worksheetName, int numberOfItems, List<ColumnDataGenerationOption> columnDataGenerationOptions);
    }
}
