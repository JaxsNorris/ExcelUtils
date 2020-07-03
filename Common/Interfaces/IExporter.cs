using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IExporter<T>
    {
        Task SaveData(List<T> data, string fullpath, string sheetname);
    }
}
