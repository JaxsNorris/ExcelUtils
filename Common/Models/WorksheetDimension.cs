namespace Common.Models
{
    public class WorksheetDimension
    {
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }

        public WorksheetDimension(int startRow, int endRow, int startColumn, int endColumn)
        {
            StartColumn = startColumn;
            EndColumn = endColumn;
            StartRow = startRow;
            EndRow = endRow;
        }
    }
}
