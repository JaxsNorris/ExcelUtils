using System.Collections.Generic;

namespace Common.Models
{
    public class AnalyserResult
    {
        public List<ColumnAnalyserResult> ColumnResults { get; set; } = new List<ColumnAnalyserResult>();
    }
}
