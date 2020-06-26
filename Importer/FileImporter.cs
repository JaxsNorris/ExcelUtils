
using Common.Attributes;
using Common.Interfaces.Parsers;
using Common.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Importer
{
    internal class FileImporter<T>
    {
        private readonly Dictionary<PropertyInfo, ExcelImportDetailAttribute> _propertyLookup;
        private readonly IParserService _parserService;

        public FileImporter(IParserService parserService)
        {
            _parserService = parserService;
            //todo throw exception if lookup empty?
            _propertyLookup = typeof(T).CreateLookupDictionary<ExcelImportDetailAttribute>();
        }

        public IEnumerable<T> Import(string fullpath, string worksheetName, int dataRowStart, int numberOfItems)
        {
            using (var package = new ExcelPackage(new FileInfo(fullpath)))
            {
                var worksheet = package.Workbook.Worksheets[worksheetName];
                if (worksheet == null)
                    throw new Exception("Worksheet not found");//todo throw custom

                var row = dataRowStart;
                for (int i = 0; i < numberOfItems; i++)
                {
                    yield return GetPopulatedItem(worksheet, row);
                    row++;
                }
            }
        }

        private T GetPopulatedItem(ExcelWorksheet worksheet, int row)
        {
            var instance = Activator.CreateInstance<T>();
            foreach (var lookup in _propertyLookup)
            {
                var address = ExcelHelper.GetAddress(lookup.Value.Column, row);
                var value = GetValue(lookup.Key.PropertyType, worksheet, address);
                lookup.Key.SetValue(instance, value);
            }
            return instance;
        }

        private object? GetValue(Type expectedType, ExcelWorksheet worksheet, string address)
        {
            var value = worksheet.Cells[address].Value;
            return _parserService.GetValue(address, value, expectedType);
        }
    }
}
