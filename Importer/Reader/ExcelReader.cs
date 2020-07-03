using Common.Exceptions;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Importer.Reader
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
            _worksheetDimension = new WorksheetDimension(dimension.Start.Row, dimension.End.Row, dimension.Start.Column, dimension.End.Column);
            return _worksheetDimension;
        }

        public ExcelCellValue GetCellValue(string address)
        {
            return GetCellValue(_worksheet.Cells[address]);
        }

        public ExcelCellValue GetCellValue(int row, int column)
        {
            return GetCellValue(_worksheet.Cells[row, column]);
        }

        public ExcelCellValue GetCellValue(int row, string column)
        {
            var address = ExcelHelper.GetAddress(row, column);
            return GetCellValue(address);
        }

        private ExcelCellValue GetCellValue(ExcelRangeBase cell)
        {
            return new ExcelCellValue(cell.Address, cell.Start.Row, cell.Start.Column, cell.Value, cell.Text);
        }

        public IEnumerable<ExcelCellValue> GetNotNullColumnValues(int startRow, int column)
        {
            return GetNotNullColumnValues(startRow, _worksheet.Dimension.End.Row, column);
        }

        public IEnumerable<ExcelCellValue> GetNotNullColumnValues(int startRow, int endRow, int column)
        {
            //Epplus filters out most nulls unless the column has a format setup
            return _worksheet.Cells[startRow, column, endRow, column]
                .Where(x => x.Value != null || !string.IsNullOrWhiteSpace(x.Text))
                .Select(x => GetCellValue(x));
        }
    }
}
