using Common.Models;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAnalyser
    {
        Task<AnalyserResult> Analyses(string fullpath, string worksheetName, int headerRow = 1);
    }
}
