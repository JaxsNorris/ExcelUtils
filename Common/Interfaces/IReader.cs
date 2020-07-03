using Common.Models;
using System;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IReader : IDisposable
    {
        public WorksheetDimension GetWorksheetDimension();
        public ExcelCellValue GetCellValue(string address);
        public ExcelCellValue GetCellValue(int row, int column);
        public ExcelCellValue GetCellValue(int row, string column);
        public IEnumerable<ExcelCellValue> GetNotNullColumnValues(int startRow, int column);
        public IEnumerable<ExcelCellValue> GetNotNullColumnValues(int startRow, int endRow, int column);
    }
}
