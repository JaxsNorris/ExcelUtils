using Analyser;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tests.Common;

namespace AnalyserTests
{
    public class AnalyserTests
    {
        private const string DefaultTestPath = "path to file";

        private ExcelAnalyser CreateFileAnalyser(IMock<IReaderFactory> readerFactoryMock = null)
        {
            var doubleParser = new DoubleParser();
            var lookupParser = new LookupParser();
            var parserService = new ParserService(new DateTimeParser(doubleParser), doubleParser, lookupParser, new BooleanParser(lookupParser), new EnumParser());

            if (readerFactoryMock == null)
                readerFactoryMock = new Mock<IReaderFactory>();

            return new ExcelAnalyser(readerFactoryMock.Object, parserService);
        }

        private IMock<IReaderFactory> CreateReaderFactoryMock(string headerValue, List<ExcelCellValue> columnValues = null, int numberOfEmptyRows = 0)
        {
            const int column = 1;
            const int numberOfColums = 1;
            const int startRow = 1;
            int numberOfRows = columnValues != null ? columnValues.Count + 1 : 5;
            return CreateReaderFactoryMock(column, numberOfColums, startRow, numberOfRows, headerValue, columnValues, numberOfEmptyRows);
        }

        private IMock<IReaderFactory> CreateReaderFactoryMock(int startColumn, int numberOfColumns, int startRow, int numberOfRows, string headerValue, List<ExcelCellValue> columnValues = null, int numberOfEmptyRows = 0)
        {
            var columnAddresss = ExcelCellAddress.GetColumnLetter(startColumn);
            var headerColumnAddress = ExcelHelper.GetAddress(startRow, columnAddresss);
            var readerMock = new Mock<IReader>();
            var readerFactoryMock = new Mock<IReaderFactory>();

            readerFactoryMock.Setup(mock => mock.CreateReader(It.IsAny<FileInfo>(), TestConstants.DefaultWorksheet))
                               .Returns(readerMock.Object);

            var totalRows = startRow + numberOfEmptyRows + numberOfRows - 1;
            readerMock.Setup(mock => mock.GetWorksheetDimension())
                .Returns(new WorksheetDimension(startRow, totalRows, startColumn, startColumn + numberOfColumns - 1));

            readerMock.Setup(mock => mock.GetCellValue(startRow, It.IsAny<int>()))
                        .Returns(new ExcelCellValue(headerColumnAddress, startRow, startColumn, headerValue, headerValue));

            readerMock.Setup(mock => mock.GetNotNullColumnValues(It.IsAny<int>(), It.IsAny<int>()))
                                        .Returns(columnValues);
            readerMock.Setup(mock => mock.GetNotNullColumnValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                        .Returns(columnValues);

            return readerFactoryMock;
        }

        private ExcelCellValue CreateExcelCellValue(int row, object value, string textValue)
        {
            var cellAddress = ExcelHelper.GetAddress(row, TestConstants.DefaultColumnAddress);
            return new ExcelCellValue(cellAddress, row, TestConstants.DefaultColumn, value, textValue);
        }

        [Test]
        public void Analyses_When_ReaderFactoryThrowsInvalidException_ThrowException()
        {
            //Setup
            var readerFactoryMock = new Mock<IReaderFactory>();
            var analyser = CreateFileAnalyser(readerFactoryMock);

            readerFactoryMock.Setup(mock => mock.CreateReader(It.IsAny<FileInfo>(), TestConstants.DefaultWorksheet))
                                .Throws(new WorksheetNotFoundException(DefaultTestPath, TestConstants.DefaultWorksheet));

            //Act
            var exception = Assert.ThrowsAsync<WorksheetNotFoundException>(async () => await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet));

            //Assert
            Assert.AreEqual(DefaultTestPath, exception.Fullpath);
            Assert.AreEqual(TestConstants.DefaultWorksheet, exception.WorksheetName);
        }

        [Test]
        public void Analyses_When_EmptyColumHeader_ThrowException()
        {
            //Setup
            var readerFactoryMock = CreateReaderFactoryMock(null);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet));

            //Assert
            Assert.True(exception.Message.Equals("Unable to analyses a column without a header", StringComparison.InvariantCulture));
        }

        [Test]
        public async Task Analyses_When_EmptyColumn_ReturnIsEmptyColumnTrue()
        {
            //Setup
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, new List<ExcelCellValue>(), 5);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsTrue(columnResult.IsEmptyColumn);
        }

        [Test]
        public async Task Analyses_When_AllColumnTypes_ReturnAllFieldTypes()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 4;
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3, "test string", "test string"),
                CreateExcelCellValue(4, TestConstants.DefaultDateTime, TestConstants.DefaultDateTime.ToShortDateString()),
                CreateExcelCellValue(5, true, "TRUE"),
                CreateExcelCellValue(6, 123.0,"123.00")
            };

            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.All, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_DuplicateDateTimeColumnValues_ReturnReducedCountWithCorrectFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 2;
            const int numberOfDistinctRows = 3;
            var otherDateValue1 = TestConstants.DefaultDateTime.AddDays(-2);
            var otherDateValue2 = TestConstants.DefaultDateTime.AddMonths(-1);
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3, TestConstants.DefaultDateTime, TestConstants.DefaultDateTime.ToShortDateString()),
                CreateExcelCellValue(4, TestConstants.DefaultDateTime, TestConstants.DefaultDateTime.ToLongTimeString()),
                CreateExcelCellValue(5, otherDateValue1, otherDateValue1.ToShortDateString()),
                CreateExcelCellValue(5, otherDateValue2, otherDateValue2.ToShortDateString()),
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.DateTime, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_DuplicateBooleanColumnValues_ReturnReducedCountWithCorrectFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 2;
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(2, true, bool.TrueString),
                CreateExcelCellValue(3, true, bool.TrueString),
                CreateExcelCellValue(4, false, bool.FalseString),
                CreateExcelCellValue(6, false, bool.FalseString),
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Boolean, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_DoubleColumnValuesThatCanBeParsedToBool_ReturnMultipleFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 0;
            const int numberOfDistinctRows = 2;
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(2, 1, "TRUE"),
                CreateExcelCellValue(3, 0, "FALSE")
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Number | FieldTypes.Boolean, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_DoubleColumnValuesThatCanBeParsedToDateTime_ReturnMultipleFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 3;
            var otherDateValue1 = TestConstants.DefaultDateTime.AddDays(-2);
            var otherDateValue2 = TestConstants.DefaultDateTime.AddMonths(-1);
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3, TestConstants.DefaultDateTime.ToOADate(), TestConstants.DefaultDateTime.ToShortDateString()),
                CreateExcelCellValue(4, TestConstants.DefaultDateTime.ToOADate(), TestConstants.DefaultDateTime.ToLongTimeString()),
                CreateExcelCellValue(5, otherDateValue1.ToOADate(), otherDateValue1.ToShortDateString()),
                CreateExcelCellValue(5, otherDateValue2.ToOADate(), otherDateValue2.ToShortDateString())
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Number | FieldTypes.DateTime, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_DoubleColumnValuesThatTextValueIsOnlyNumber_ReturnNumberFieldTypeOnly()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 1;
            var dateDoubleValue = TestConstants.DefaultDateTime.ToOADate();
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3, dateDoubleValue, dateDoubleValue.ToString())
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Number, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_StringColumnValuesThatCanBeParsedToBool_ReturnMultipleFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 0;
            const int numberOfDistinctRows = 2;
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(2, bool.TrueString, bool.TrueString),
                CreateExcelCellValue(3, bool.FalseString, bool.FalseString)
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Text | FieldTypes.Boolean, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_When_StringColumnValuesThatCanBeParsedToDateTime_ReturnMultipleFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 3;
            var otherDateValue1 = TestConstants.DefaultDateTime.AddDays(-2);
            var otherDateValue2 = TestConstants.DefaultDateTime.AddMonths(-1);
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3, TestConstants.DefaultDateTime.ToShortDateString(), TestConstants.DefaultDateTime.ToShortDateString()),
                CreateExcelCellValue(4, TestConstants.DefaultDateTime.ToShortDateString(), TestConstants.DefaultDateTime.ToLongTimeString()),
                CreateExcelCellValue(5, otherDateValue1.ToOADate().ToString(), otherDateValue1.ToShortDateString()),
                CreateExcelCellValue(5, otherDateValue2.ToOADate().ToString(), otherDateValue2.ToShortDateString())
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Text | FieldTypes.DateTime, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

        [Test]
        public async Task Analyses_WhenStringColumnValuesThatCanBeParsedToDateTimeOrBool_ReturnMultipleFieldType()
        {
            //Setup
            const int numberOfEmptyRows = 1;
            const int numberOfDistinctRows = 2;
            var columnValues = new List<ExcelCellValue>()
            {
                CreateExcelCellValue(3,  "hello", "hello"),
                CreateExcelCellValue(4,  "test", "test"),
                CreateExcelCellValue(5,  "test", "test")
            };
            var readerFactoryMock = CreateReaderFactoryMock(TestConstants.DefaultColumnHeader, columnValues, numberOfEmptyRows);
            var analyser = CreateFileAnalyser(readerFactoryMock);

            //Act
            var result = await analyser.Analyses(DefaultTestPath, TestConstants.DefaultWorksheet);

            //Assert
            var columnResult = result.ColumnResults.FirstOrDefault();
            Assert.NotNull(columnResult);
            Assert.AreEqual(TestConstants.DefaultColumnHeader, columnResult.ColumnHeader);
            Assert.IsFalse(columnResult.IsEmptyColumn);
            Assert.AreEqual(FieldTypes.Text, columnResult.FieldTypes);
            Assert.AreEqual(columnValues.Count + numberOfEmptyRows, columnResult.TotalNumberOfRows);
            Assert.AreEqual(columnValues.Count, columnResult.NumberOfDataRows);
            Assert.AreEqual(numberOfDistinctRows, columnResult.NumberOfDistinctRows);
            Assert.AreEqual(numberOfEmptyRows, columnResult.NumberOfEmptyRows);
        }

    }
}
