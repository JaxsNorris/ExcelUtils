using Common.Enums;
using Common.Interfaces;
using Common.Interfaces.Parsers;
using Common.Models;
using Common.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Analyser
{
    public class ExcelAnalyser : IAnalyser
    {
        private const int SampleSize = 10;
        private readonly IReaderFactory _readerFactory;
        private readonly IParserService _parserService;

        public ExcelAnalyser(IReaderFactory readerFactory, IParserService parserService)
        {
            _readerFactory = readerFactory;
            _parserService = parserService;
        }

        public Task<AnalyserResult> Analyses(string fullpath, string worksheetName, int headerRow = 1)
        {
            var result = new AnalyserResult();
            using (var reader = _readerFactory.CreateReader(new FileInfo(fullpath), worksheetName))
            {
                var dimension = reader.GetWorksheetDimension();
                for (int i = dimension.StartColumn; i <= dimension.EndColumn; i++)
                {
                    result.ColumnResults.Add(GetColumnAnalyserResult(reader, headerRow, dimension.EndRow, i));
                }
            }
            return Task.FromResult(result);
        }

        private ColumnAnalyserResult GetColumnAnalyserResult(IReader reader, int headerRow, int lastRow, int column)
        {
            var headerCellValue = reader.GetCellValue(headerRow, column);
            if (string.IsNullOrWhiteSpace(headerCellValue.TextValue))
                throw new InvalidOperationException("Unable to analyses a column without a header");

            var cellsWithData = reader.GetNotNullColumnValues(headerRow + 1, column)
                                        .ToList();
            var totalRowsCount = lastRow - headerRow;

            var result = new ColumnAnalyserResult()
            {
                ColumnHeader = headerCellValue.TextValue,
                ColumnAddress = ExcelCellAddress.GetColumnLetter(column),
                TotalNumberOfRows = totalRowsCount
            };

            if (!cellsWithData.Any())
            {
                result.IsEmptyColumn = true;
                result.NumberOfEmptyRows = totalRowsCount;
                return result;
            }

            result.NumberOfDataRows = cellsWithData.Count;
            result.NumberOfEmptyRows = totalRowsCount - cellsWithData.Count;
            result.NumberOfDistinctRows = cellsWithData.Select(x => x.Value).Distinct().Count();
            result.FieldTypes = DetermineFieldType(cellsWithData);
            return result;
        }

        private FieldTypes DetermineFieldType(IEnumerable<ExcelCellValue> cellsWithValues)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var groupedByType = cellsWithValues.GroupBy(x => x.Value.GetType());
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var fieldTypes = FieldTypes.None;
            foreach (var group in groupedByType)
            {
                var checkForBoolAndDateTime = false;

                if (group.Key == typeof(bool))
                    fieldTypes |= FieldTypes.Boolean;
                else if (group.Key == typeof(DateTime))
                    fieldTypes |= FieldTypes.DateTime;
                else if (group.Key.IsNumeric())
                {
                    fieldTypes |= FieldTypes.Number;
                    checkForBoolAndDateTime = true;
                }
                else
                {
                    fieldTypes |= FieldTypes.Text;
                    checkForBoolAndDateTime = true;
                }

                if (checkForBoolAndDateTime)
                {
                    if (!fieldTypes.HasFlag(FieldTypes.Boolean) && CanBeParsedToBooleanValue(group))
                        fieldTypes |= FieldTypes.Boolean;

                    if (!fieldTypes.HasFlag(FieldTypes.DateTime) && CanBeParsedToDateTimeValue(group.Key, group))
                        fieldTypes |= FieldTypes.DateTime;
                }
            }

            return fieldTypes;
        }

        private bool CanBeParsedToBooleanValue(IEnumerable<ExcelCellValue> cellValues)
        {
            foreach (var cellValue in cellValues.Take(SampleSize))
            {
                try
                {
                    _parserService.ParseBoolean(cellValue.Value);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanBeParsedToDateTimeValue(Type valueType, IEnumerable<ExcelCellValue> cellValues)
        {
            foreach (var cellValue in cellValues.Take(SampleSize))
            {
                if (valueType.IsNumeric() && (string.IsNullOrWhiteSpace(cellValue.TextValue) || cellValue.TextValue.IsNumericString()))
                    return false;

                try
                {
                    _parserService.ParseDateTime(cellValue.Value, null);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
