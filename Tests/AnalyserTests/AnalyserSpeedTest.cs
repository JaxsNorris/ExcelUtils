using Analyser;
using Common.Models;
using Importer.Parsers;
using Importer.Reader;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tests.Common;

namespace AnalyserTests
{
    public class AnalyserSpeedTest
    {
        private ExcelAnalyser CreateExcelAnalyser()
        {
            var doubleParser = new DoubleParser();
            var lookupParser = new LookupParser();
            var parserService = new ParserService(new DateTimeParser(doubleParser), doubleParser, lookupParser, new BooleanParser(lookupParser));
            var readerFactory = new ExcelReaderFactory();

            return new ExcelAnalyser(readerFactory, parserService);
        }

        [TestCase("SmallDataFile.xlsx", 3, ExpectedSmallDataFileResults.Json)]
        [TestCase("MediumDataFile.xlsx", 5, ExpectedMediumDataFileResults.Json)]
        //[TestCase("BigDataFile.xlsx", 60, ExpectedBigDataFileResults.Json)] //this test case takes a while since it is processing half a million records so it is only needed to be run when something changes on the analyzer
        public async Task DoSpeedTest(string filename, int expectTimeInSeconds, string expectedResultJson)
        {
            var analyser = CreateExcelAnalyser();
            var path = ResourceFileHelper.GetPath(filename);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await analyser.Analyses(path, "data");
            stopwatch.Stop();

            Assert.LessOrEqual(stopwatch.Elapsed.TotalSeconds, TimeSpan.FromSeconds(expectTimeInSeconds).TotalSeconds);
            Assert.NotNull(result);
            var expectedResult = JsonConvert.DeserializeObject<AnalyserResult>(expectedResultJson);
            Assert.AreEqual(expectedResult.ColumnResults.Count, result.ColumnResults.Count);
            foreach (var columnResult in result.ColumnResults)
            {
                var exptectColumnResult = expectedResult.ColumnResults.SingleOrDefault(x => x.ColumnHeader.Equals(columnResult.ColumnHeader));
                Assert.NotNull(exptectColumnResult);
                Assert.AreEqual(exptectColumnResult.IsEmptyColumn, columnResult.IsEmptyColumn);
                Assert.AreEqual(exptectColumnResult.FieldTypes, columnResult.FieldTypes);
                Assert.AreEqual(exptectColumnResult.TotalNumberOfRows, columnResult.TotalNumberOfRows);
                Assert.AreEqual(exptectColumnResult.NumberOfDataRows, columnResult.NumberOfDataRows);
                Assert.AreEqual(exptectColumnResult.NumberOfDistinctRows, columnResult.NumberOfDistinctRows);
                Assert.AreEqual(exptectColumnResult.NumberOfEmptyRows, columnResult.NumberOfEmptyRows);
            }
        }
    }
}
