using Common.Enums;

namespace Common.Models
{
    public class ColumnAnalyserResult
    {
        public string ColumnHeader { get; set; } = string.Empty;
        public int TotalNumberOfRows { get; set; }
        public int NumberOfDataRows { get; set; }
        public int NumberOfDistinctRows { get; set; }
        public int NumberOfEmptyRows { get; set; }
        public string ColumnAddress { get; set; } = string.Empty;
        public FieldTypes FieldTypes { get; set; }
        public bool IsEmptyColumn { get; set; }
    }
}
