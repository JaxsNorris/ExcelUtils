using Common.Exceptions;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reader
{
    internal sealed class ExcelReader : IReader
    {
        private readonly ExcelPackage _package;
        private readonly ExcelWorksheet _worksheet;
        private WorksheetDimension _worksheetDimension;

        public ExcelReader(FileInfo file, string worksheetName)
        {
            _package = new ExcelPackage(file);
            _worksheet = _package.Workbook.Worksheets[worksheetName];
            if (_worksheet == null)
                throw new WorksheetNotFoundException(file.FullName, worksheetName);
        }

        public void Dispose()
        {
            _worksheet.Dispose();
            _package.Dispose();
        }

        public WorksheetDimension GetWorksheetDimension()
        {
            if (_worksheetDimension != null)
                return _worksheetDimension;

            var dimension = _worksheet.Dimension;
            _worksheetDimension = new WorksheetDimension(dimension.Start.Column, dimension.End.Column, dimension.Start.Row, dimension.End.Row);
            return _worksheetDimension;
        }

        public ExcelCellValue GetCellValue(string address)
        {
            return GetCellValue(_worksheet.Cells[address]);
        }

        public ExcelCellValue GetCellValue(int column, int row)
        {
            return GetCellValue(_worksheet.Cells[row, column]);
        }

        public ExcelCellValue GetCellValue(string column, int row)
        {
            var address = ExcelHelper.GetAddress(column, row);
            return GetCellValue(address);
        }

        private ExcelCellValue GetCellValue(ExcelRangeBase cell)
        {
            return new ExcelCellValue(cell.Address, cell.Start.Row, cell.Start.Column, cell.Value, cell.Text);
        }

        public IEnumerable<ExcelCellValue> GetColumnValues(int column, int startRow)
        {
            return GetColumnValues(column, startRow, _worksheet.Dimension.End.Row);
        }

        public IEnumerable<ExcelCellValue> GetColumnValues(int column, int startRow, int endRow)
        {
            return _worksheet.Cells[startRow, column, endRow, column].Select(x => GetCellValue(x));
        }
    }
}
