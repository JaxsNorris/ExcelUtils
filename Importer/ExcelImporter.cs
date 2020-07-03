using Common.Attributes;
using Common.Exceptions;
using Common.Interfaces;
using Common.Interfaces.Parsers;
using Common.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Importer
{
    internal class ExcelImporter<TOut> : IImporter<TOut>
    {
        private readonly Dictionary<PropertyInfo, ExcelImportDetailAttribute> _propertyLookup;
        private readonly IParserService _parserService;

        public ExcelImporter(IParserService parserService)
        {
            _parserService = parserService;
            //todo throw exception if lookup empty?
            _propertyLookup = typeof(TOut).CreateLookupDictionary<ExcelImportDetailAttribute>();
        }

        public IEnumerable<TOut> Import(string fullpath, string worksheetName, int dataRowStart, int numberOfItems)
        {
            using (var package = new ExcelPackage(new FileInfo(fullpath)))
            {
                var worksheet = package.Workbook.Worksheets[worksheetName];
                if (worksheet == null)
                    throw new WorksheetNotFoundException(fullpath, worksheetName);

                var row = dataRowStart;
                for (int i = 0; i < numberOfItems; i++)
                {
                    yield return GetPopulatedItem(worksheet, row);
                    row++;
                }
            }
        }

        private TOut GetPopulatedItem(ExcelWorksheet worksheet, int row)
        {
            var instance = Activator.CreateInstance<TOut>();
            foreach (var lookup in _propertyLookup)
            {
                var address = ExcelHelper.GetAddress(row, lookup.Value.Column);
                var value = GetValue(lookup.Key.PropertyType, worksheet, address);
                lookup.Key.SetValue(instance, value);
            }
            return instance;
        }

        private object? GetValue(Type expectedType, ExcelWorksheet worksheet, string address)
        {
            var value = worksheet.Cells[address].Value;
            return _parserService.GetValue(value, expectedType);
        }
    }
}
