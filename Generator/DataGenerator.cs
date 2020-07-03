using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class DataGenerator : IDataGenerator
    {
        private static readonly Random Random = new Random();
        public Task GenerateData(string fullpath, string worksheetName, int numberOfItems, List<ColumnDataGenerationOption> columnDataGenerationOptions)
        {
            using (var package = new ExcelPackage(new FileInfo(fullpath)))
            {
                var worksheet = CreateOrReplaceWorksheet(package, worksheetName);
                var columnNumber = 1;
                foreach (var columnDataGeneration in columnDataGenerationOptions)
                {
                    var columnAddress = ExcelCellAddress.GetColumnLetter(columnNumber);
                    SetHeaderValueWithStyle(worksheet, ExcelHelper.GetAddress(1, columnAddress), columnDataGeneration.CoulmnHeader);
                    AddColumnData(worksheet, numberOfItems, columnNumber, columnDataGeneration);
                    columnNumber++;
                }
                package.Save();
            }
            return Task.CompletedTask;
        }

        private ExcelWorksheet CreateOrReplaceWorksheet(ExcelPackage package, string sheetName)
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet != null)
                package.Workbook.Worksheets.Delete(worksheet);

            worksheet = package.Workbook.Worksheets.Add(sheetName);
            return worksheet;
        }

        private static void SetHeaderValueWithStyle(ExcelWorksheet worksheet, string address, string header)
        {
            var cell = worksheet.Cells[address];
            cell.Value = header;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Font.Color.SetColor(Color.White);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#007F92"));
            cell.AutoFitColumns();
        }

        private void AddColumnData(ExcelWorksheet worksheet, int numberOfItems, int column, ColumnDataGenerationOption columnDataGeneration)
        {
            for (int rowNumber = 2; rowNumber <= numberOfItems + 1; rowNumber++)
            {
                object? value;
                if (columnDataGeneration.Values != null)
                    value = GetRandomLookup(columnDataGeneration.Values);
                else
                {
                    if (columnDataGeneration.OtherFieldTypes == null)
                        value = GetRandomValue(columnDataGeneration.ColumnFieldType);
                    else
                    {
                        var pos = GetRandomArrayPosition(columnDataGeneration.OtherFieldTypes.Length);
                        value = GetRandomValue(columnDataGeneration.OtherFieldTypes[pos]);
                    }
                }

                if (value is DateTime dateTimeValue)
                {
                    worksheet.Cells[rowNumber, column].Style.Numberformat.Format = "yyyy/mm/dd";
                    worksheet.Cells[rowNumber, column].Value = dateTimeValue;
                }
                else
                    worksheet.SetValue(rowNumber, column, value);
            }

            if (columnDataGeneration.HasBlankRows)
                InsertClearRandomCells(worksheet, numberOfItems, column);
        }

        private void InsertClearRandomCells(ExcelWorksheet worksheet, int totalRows, int column)
        {
            int numberOfBlanks;
            if (totalRows < 150)
                numberOfBlanks = Random.Next(1, 10);
            else
            {
                var percentage = totalRows / 100;
                numberOfBlanks = Random.Next(percentage, percentage * 5); //from 1% 5% can be blank
            }

            for (int i = 0; i < numberOfBlanks; i++)
            {
                var row = GetRandomArrayPosition(totalRows);
                worksheet.SetValue(row, column, null);
            }
        }

        private object? GetRandomValue(FieldTypes columnFieldType)
        {
            switch (columnFieldType)
            {
                case FieldTypes.None:
                    return null;
                case FieldTypes.Number:
                    return GetRandomNumber();
                case FieldTypes.Boolean:
                    return GetRandomBoolean();
                case FieldTypes.DateTime:
                    return GetRandomDate();
                default:
                    return RandomString();
            }
        }

        private int GetRandomPositiveSmallNumber()
        {
            return Random.Next(1, 50);
        }

        private double GetRandomNumber()
        {
            return Random.Next(int.MinValue, int.MaxValue);
        }

        private bool GetRandomBoolean()
        {
            return Random.Next(0, 2) == 1;
        }

        private DateTime GetRandomDate()
        {
            var dateDouble = Random.Next(7306, 80720); // OaDate value from 1920 to 2120
            return DateTime.FromOADate(dateDouble);
        }

        private string RandomString()
        {
            int size = GetRandomPositiveSmallNumber();
            var builder = new StringBuilder(size);

            char offset = 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++)
            {
                var @char = (char)Random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return builder.ToString();
        }

        private object GetRandomLookup(object[] values)
        {
            return values[GetRandomArrayPosition(values.Length)];
        }

        private int GetRandomArrayPosition(int size)
        {
            return Random.Next(0, size);
        }
    }
}
