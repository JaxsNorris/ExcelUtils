using Common.Exceptions;
using Common.Interfaces.Parsers;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using System;

namespace ImporterTests.Parsers
{
    public class DateTimeParserTests
    {
        private readonly DateTime _expectedDate = new DateTime(2020, 6, 4);
        private const double OleDateDoubleValueForExpectedDate = 43986;

        private DateTimeParser CreateDateTimeParser(IMock<IDoubleParser> doubleParserMock = null)
        {
            if (doubleParserMock == null)
            {
                doubleParserMock = new Mock<IDoubleParser>();
            }

            return new DateTimeParser(doubleParserMock.Object);
        }

        [Test]
        public void Parse_WhenNull_ReturnNullValue()
        {
            var parser = CreateDateTimeParser();

            var parsedValue = parser.Parse(null, null);

            Assert.Null(parsedValue);
        }

        [Test]
        public void Parse_WhenValidDateTime_ReturnCorrectDateValue()
        {
            var parser = CreateDateTimeParser();

            var parsedValue = parser.Parse(_expectedDate, null);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(_expectedDate, parsedValue.Value);
        }

        [TestCase(true)]
        [TestCase('t')]
        public void Parse_WhenNotSupportedType_ThrowException(object invalidTypeValue)
        {
            var parser = CreateDateTimeParser();

            var exception = Assert.Throws<UnsupportedDataTypeParserException>(() => parser.Parse(invalidTypeValue, null));

            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains(invalidTypeValue.GetType().Name));
        }

        #region String Value

        [TestCase("2020/06/04")]
        [TestCase("20/06/04")]
        [TestCase("2020-06-04")]
        [TestCase("4 June 2020")]
        public void Parse_WhenValidDateStringsAndNullFormat_ReturnCorrectDateValue(string dateString)
        {
            var parser = CreateDateTimeParser();

            var parsedValue = parser.Parse(dateString, null);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(_expectedDate, parsedValue.Value);
        }

        [TestCase("2020/06/04", "yyyy/MM/dd")]
        [TestCase("2020/6/4", "yyyy/M/d")]
        [TestCase("20/06/04", "yy/MM/dd")]
        [TestCase("20/6/4", "yy/M/d")]
        [TestCase("2020-06-04", "yyyy-MM-dd")]
        [TestCase("04 June 2020", "dd MMMM yyyy")]
        [TestCase("4 June 2020", "d MMMM yyyy")]
        [TestCase("2020/04/06", "yyyy/dd/MM")]
        [TestCase("20/04/06", "yy/dd/MM")]
        [TestCase("2020-04-06", "yyyy-dd-MM")]
        public void Parse_WhenValidDateStringsWithFormat_ReturnCorrectDateValue(string dateString, string dateFormat)
        {
            var parser = CreateDateTimeParser();

            var parsedValue = parser.Parse(dateString, new string[] { dateFormat });

            Assert.NotNull(parsedValue);
            Assert.AreEqual(_expectedDate, parsedValue.Value);
        }

        [TestCase("Hello World")]
        [TestCase("Hello World", null)]
        [TestCase("Hello World", "yyyy/MM/dd")]
        public void Parse_WhenInvalidDateStringAndNotVaildDouble_ThrowNewParserException(string invalidDateString, params string[] dateFormats)
        {
            var doubleParsingException = ParserException.CreateWithEnrichedMessage(invalidDateString, "double", null, "Invalid double");
            var doubleParserMock = new Mock<IDoubleParser>();
            var parser = CreateDateTimeParser(doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(invalidDateString))
              .Throws(doubleParsingException);

            var exception = Assert.Throws<ParserException>(() => parser.Parse(invalidDateString, dateFormats));

            Assert.AreNotEqual(doubleParsingException, exception);
            Assert.Null(exception.InnerException);

            var exceptionContainsFormats = exception.Message.Contains("with the format(s)");
            if (dateFormats == null || dateFormats.Length == 0)
            {
                Assert.IsFalse(exceptionContainsFormats);
            }
            else
            {
                Assert.IsTrue(exceptionContainsFormats);
            }

            doubleParserMock.Verify(mock => mock.Parse(invalidDateString), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }

        #endregion

        #region OA Date Double

        [Test]
        public void Parse_WhenCorrectOADouble_ReturnCorrectDateValue()
        {
            var parser = CreateDateTimeParser();
            var parsedValue = parser.Parse(OleDateDoubleValueForExpectedDate, null);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(_expectedDate, parsedValue.Value);
        }

        [Test]
        public void Parse_WhenCorrectOAString_ReturnCorrectDateValue()
        {
            var oleStringValue = OleDateDoubleValueForExpectedDate.ToString();
            var doubleParserMock = new Mock<IDoubleParser>();
            var parser = CreateDateTimeParser(doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(oleStringValue))
              .Returns(OleDateDoubleValueForExpectedDate);

            var parsedValue = parser.Parse(oleStringValue, null);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(_expectedDate, parsedValue.Value);
            doubleParserMock.Verify(mock => mock.Parse(oleStringValue), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }

        [TestCase(2958466)]
        [TestCase(-657435)]
        public void Parse_WhenInvalidOADoubleValue_ThrowParserExceptionWithInnerException(double invalidOleDate)
        {
            var doubleParserMock = new Mock<IDoubleParser>();
            var parser = CreateDateTimeParser(doubleParserMock);

            var exception = Assert.Throws<ParserException>(() => parser.Parse(invalidOleDate, null));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual("Not a legal OleAut date.", exception.InnerException.Message);
            doubleParserMock.VerifyNoOtherCalls();
        }

        [TestCase("2958466", 2958466)]
        [TestCase("-657435", -657435)]
        public void Parse_WhenInvalidOAStringValue_ThrowParserExceptionWithInnerException(string invalidOleDate, double returnedValue)
        {
            var doubleParserMock = new Mock<IDoubleParser>();
            var parser = CreateDateTimeParser(doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(invalidOleDate))
              .Returns(returnedValue);

            var exception = Assert.Throws<ParserException>(() => parser.Parse(invalidOleDate, null));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual("Not a legal OleAut date.", exception.InnerException.Message);
            doubleParserMock.Verify(mock => mock.Parse(invalidOleDate), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }
        #endregion
    }
}
