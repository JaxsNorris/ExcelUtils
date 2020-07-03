﻿
using Common.Interfaces;
using System.IO;

namespace Importer.Reader
{
    internal class ExcelReaderFactory : IReaderFactory
    {
        public IReader CreateReader(FileInfo file, string worksheetName)
        {
            return new ExcelReader(file, worksheetName);
        }
    }
}