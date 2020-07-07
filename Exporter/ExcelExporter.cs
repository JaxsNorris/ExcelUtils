
using Common.Attributes;
using Common.Extensions;
using Common.Interfaces;
using Common.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Exporter
{
    internal class ExcelExporter<T> : IExporter<T>
    {
        private const string DefaultHeaderColorBlue = "#007F92"; //todo move this out to make it configurable
        private readonly Dictionary<PropertyInfo, ExcelExportDetailAttribute> _propertyLookup;

        public ExcelExporter()
        {
            _propertyLookup = typeof(T).CreateLookupDictionary<ExcelExportDetailAttribute>();
        }

        public Task SaveData(List<T> data, string fullpath, string sheetname)
        {
            using (var package = new ExcelPackage(new FileInfo(fullpath)))
            {
                var worksheet = GetOrCreateInputWorksheet(sheetname, package);
                var rowNum = 2;
                foreach (var item in data)
                {
                    SetInputValues(worksheet, item, rowNum);
                    rowNum++;
                }
                package.Save();
            }
            return Task.CompletedTask;
        }

        private ExcelWorksheet GetOrCreateInputWorksheet(string sheetName, ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet != null)
                return worksheet;

            worksheet = package.Workbook.Worksheets.Add(sheetName);
            AddInputHeader(worksheet);
            return worksheet;
        }

        private void AddInputHeader(ExcelWorksheet worksheet)
        {
            foreach (var lookup in _propertyLookup)
            {
                var address = ExcelHelper.GetAddress(1, lookup.Value.ColumnAddress);
                var header = lookup.Value.ColumnHeader;
                if (string.IsNullOrWhiteSpace(header))
                {
                    header = lookup.Key.Name.FromCamelCase();
                }
                worksheet.SetValue(address, header);
                SetHeaderStyle(worksheet, address, lookup.Value.ColumnColorHex.GetValueOrDefault(DefaultHeaderColorBlue), lookup.Value.IsHidden);
            }
        }

        private static void SetHeaderStyle(ExcelWorksheet worksheet, string address, string colorHex, bool isHidden)
        {
            var cell = worksheet.Cells[address];
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Font.Color.SetColor(Color.White);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorHex));
            cell.AutoFitColumns();
            worksheet.Column(cell.Start.Column).Hidden = isHidden;
        }

        private void SetInputValues(ExcelWorksheet ws, T item, int rowNum)
        {
            foreach (var lookup in _propertyLookup)
            {
                var value = lookup.Key.GetValue(item);
                ws.SetValue(ExcelHelper.GetAddress(rowNum, lookup.Value.ColumnAddress), value);
            }
        }
    }
}
