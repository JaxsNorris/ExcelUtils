
using Common.Interfaces.Parsers;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tests.Common;

namespace ImporterTests.Parsers
{
    public class ParserServiceTests
    {
        private ParserService CreateParserService(IMock<IDoubleParser> doubleParserMock = null, IMock<IDateTimeParser> dateTimeParserMock = null, IMock<ILookupParser> lookupParserMock = null)
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

            return new ParserService(dateTimeParserMock.Object, doubleParserMock.Object, lookupParserMock.Object);
        }

        [Test]
        public void GetValue_WhenNullValue_ReturnNull()
        {
            var parserService = CreateParserService();

            var returnValue = parserService.GetValue(TestConstants.DefaultFullAddress, null, typeof(string));

            Assert.IsNull(returnValue);
        }

        [TestCase("Hello")]
        [TestCase(double.MaxValue)]
        [TestCase(false)]
        //[TestCase(DateTime.MinValue)]
        public void GetValue_WhenValueMatchRequired_ReturnValue(object inputValue)
        {
            var parserService = CreateParserService();

            var returnValue = parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, inputValue.GetType());

            Assert.AreEqual(inputValue, returnValue);
        }

        [Test]
        public void GetValue_WhenDoubleValue_ReturnValueFromDoubleParser()
        {
            var inputValue = "42.88";
            var expectedOutputValue = 42.88;
            var doubleParserMock = new Mock<IDoubleParser>();
            var parserService = CreateParserService(doubleParserMock: doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(TestConstants.DefaultFullAddress, inputValue))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, typeof(double));

            Assert.AreEqual(expectedOutputValue, returnValue);

            doubleParserMock.Verify(mock => mock.Parse(TestConstants.DefaultFullAddress, inputValue), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenDateTimeValue_ReturnValueFromDateTimeParser()
        {
            var expectedOutputValue = new DateTime(2020, 6, 4);
            var inputValue = expectedOutputValue.ToShortDateString();
            var dateTimeParserMock = new Mock<IDateTimeParser>();
            var parserService = CreateParserService(dateTimeParserMock: dateTimeParserMock);
            dateTimeParserMock.Setup(mock => mock.Parse(TestConstants.DefaultFullAddress, inputValue, It.IsAny<string[]>()))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, typeof(DateTime));

            Assert.AreEqual(expectedOutputValue, returnValue);

            dateTimeParserMock.Verify(mock => mock.Parse(TestConstants.DefaultFullAddress, inputValue, It.IsAny<string[]>()), Times.Once);
            dateTimeParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanStringValue_ReturnValueFromLookupParser()
        {
            var expectedOutputValue = false;
            var inputValue = "false";
            var lookupParserMock = new Mock<ILookupParser>();
            var parserService = CreateParserService(lookupParserMock: lookupParserMock);
            lookupParserMock.Setup(mock => mock.Parse(inputValue, It.IsAny<IReadOnlyDictionary<bool, string[]>>(), null, null))
                .Returns(expectedOutputValue);

            var returnValue = parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, typeof(bool));

            Assert.AreEqual(expectedOutputValue, returnValue);

            lookupParserMock.Verify(mock => mock.Parse(inputValue, It.IsAny<IReadOnlyDictionary<bool, string[]>>(), null, null), Times.Once);
            lookupParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanNotStringValue_ThrowException()
        {
            var inputValue = double.MaxValue;
            var parserService = CreateParserService();

            Assert.Throws<NotSupportedException>(() => parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, typeof(bool)));
        }

        [Test]
        public void GetValue_WhenNotSupportedExceptionType_ThrowException()
        {
            var inputValue = int.MaxValue;
            var parserService = CreateParserService();

            Assert.Throws<NotSupportedException>(() => parserService.GetValue(TestConstants.DefaultFullAddress, inputValue, typeof(bool)));
        }
    }
}
