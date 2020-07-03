
using Common.Exceptions;
using Common.Interfaces.Parsers;
using Common.Models;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using System;

namespace ImporterTests.Parsers
{
    public class ParserServiceTests
    {
        private ParserService CreateParserService(IMock<IDoubleParser> doubleParserMock = null, IMock<IDateTimeParser> dateTimeParserMock = null, IMock<ILookupParser> lookupParserMock = null, IMock<IBooleanParser> booleanParserMock = null)
        {
            if (doubleParserMock == null)
            {
                doubleParserMock = new Mock<IDoubleParser>();
            }
            if (dateTimeParserMock == null)
            {
                dateTimeParserMock = new Mock<IDateTimeParser>();
            }
            if (lookupParserMock == null)
            {
                lookupParserMock = new Mock<ILookupParser>();
            }
            if (booleanParserMock == null)
            {
                booleanParserMock = new Mock<IBooleanParser>();
            }

            return new ParserService(dateTimeParserMock.Object, doubleParserMock.Object, lookupParserMock.Object, booleanParserMock.Object);
        }

        [Test]
        public void GetValue_WhenNullValue_ReturnNull()
        {
            var parserService = CreateParserService();

            var returnValue = parserService.GetValue(null, typeof(string));

            Assert.IsNull(returnValue);
        }

        [TestCase("Hello")]
        [TestCase(double.MaxValue)]
        [TestCase(false)]
        //[TestCase(DateTime.MinValue)]
        public void GetValue_WhenValueMatchRequired_ReturnValue(object inputValue)
        {
            var parserService = CreateParserService();

            var returnValue = parserService.GetValue(inputValue, inputValue.GetType());

            Assert.AreEqual(inputValue, returnValue);
        }

        [Test]
        public void GetValue_WhenDoubleValue_ReturnValueFromDoubleParser()
        {
            var inputValue = "42.88";
            var expectedOutputValue = 42.88;
            var doubleParserMock = new Mock<IDoubleParser>();
            var parserService = CreateParserService(doubleParserMock: doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(inputValue))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(inputValue, typeof(double));

            Assert.AreEqual(expectedOutputValue, returnValue);

            doubleParserMock.Verify(mock => mock.Parse(inputValue), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenDateTimeValue_ReturnValueFromDateTimeParser()
        {
            var expectedOutputValue = new DateTime(2020, 6, 4);
            var inputValue = expectedOutputValue.ToShortDateString();
            var dateTimeParserMock = new Mock<IDateTimeParser>();
            var parserService = CreateParserService(dateTimeParserMock: dateTimeParserMock);
            dateTimeParserMock.Setup(mock => mock.Parse(inputValue, It.IsAny<string[]>()))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(inputValue, typeof(DateTime));

            Assert.AreEqual(expectedOutputValue, returnValue);

            dateTimeParserMock.Verify(mock => mock.Parse(inputValue, It.IsAny<string[]>()), Times.Once);
            dateTimeParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanStringValue_ReturnValueFromLookupParser()
        {
            var expectedOutputValue = false;
            var inputValue = "false";
            var booleanParserMock = new Mock<IBooleanParser>();
            var parserService = CreateParserService(booleanParserMock: booleanParserMock);
            booleanParserMock.Setup(mock => mock.Parse(inputValue))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(inputValue, typeof(bool));

            Assert.AreEqual(expectedOutputValue, returnValue);

            booleanParserMock.Verify(mock => mock.Parse(inputValue), Times.Once);
            booleanParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanParserThrowsException_ThrowException()
        {
            var inputValue = double.MaxValue;
            var booleanParserMock = new Mock<IBooleanParser>();
            var parserService = CreateParserService(booleanParserMock: booleanParserMock);
            booleanParserMock.Setup(mock => mock.Parse(inputValue))
                .Throws(ParserException.CreateUnsupportedValueException(inputValue, "boolean"));

            Assert.Throws<ParserException>(() => parserService.GetValue(inputValue, typeof(bool)));
        }

        [Test]
        public void GetValue_WhenNotSupportedExceptionType_ThrowException()
        {
            var inputValue = int.MaxValue;
            var parserService = CreateParserService();

            Assert.Throws<UnsupportedDataTypeParserException>(() => parserService.GetValue(inputValue, typeof(Movie)));
        }
    }
}
