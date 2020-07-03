using Common.Models;
using Exporter;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Tests.Common;

namespace ExporterTests
{
    public class FileExporterTests
    {
        private ExcelExporter<Movie> GetFileExporter()
        {
            return new ExcelExporter<Movie>();
        }

        [Test]
        public async Task SaveData_When_ValidDate_OutputFile()
        {
            var exporter = GetFileExporter();
            var path = ResourceFileHelper.GetPath("ExportedMovieData.xlsx");
            var sheetname = $"Test-{DateTime.UtcNow.ToString("MM-dd-yy H_mm_ss")}";
            await exporter.SaveData(DataGenerator.GetMovieList(), path, sheetname);
            Assert.IsTrue(File.Exists(path));
            //todo finish this test
        }
    }
}
