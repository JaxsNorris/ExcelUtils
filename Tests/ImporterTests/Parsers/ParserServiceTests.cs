
using Common.Exceptions;
using Common.Interfaces.Parsers;
using Common.Models;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using System;
using Tests.Common.TestClasses;

namespace ImporterTests.Parsers
{
    public class ParserServiceTests
    {
        private ParserService CreateParserService(IMock<IDoubleParser> doubleParserMock = null,
                                                    IMock<IDateTimeParser> dateTimeParserMock = null,
                                                    IMock<ILookupParser> lookupParserMock = null,
                                                    IMock<IBooleanParser> booleanParserMock = null,
                                                    IMock<IEnumParser> enumParserMock = null)
        {
            doubleParserMock ??= new Mock<IDoubleParser>();
            dateTimeParserMock ??= new Mock<IDateTimeParser>();
            lookupParserMock ??= new Mock<ILookupParser>();
            booleanParserMock ??= new Mock<IBooleanParser>();
            enumParserMock ??= new Mock<IEnumParser>();
            return new ParserService(dateTimeParserMock.Object, doubleParserMock.Object, lookupParserMock.Object, booleanParserMock.Object, enumParserMock.Object);
        }

        [Test]
        public void GetValue_WhenNullValue_ReturnNull()
        {
            //Arrange
            var parserService = CreateParserService();

            //Act
            var returnValue = parserService.GetValue(null, typeof(string));

            //Assert
            Assert.IsNull(returnValue);
        }

        [TestCase("Hello")]
        [TestCase(double.MaxValue)]
        [TestCase(false)]
        //[TestCase(DateTime.MinValue)]
        public void GetValue_WhenValueTypeMatch_ReturnValue(object inputValue)
        {
            //Arrange
            var parserService = CreateParserService();

            //Act
            var returnValue = parserService.GetValue(inputValue, inputValue.GetType());

            //Assert
            Assert.AreEqual(inputValue, returnValue);
        }

        [Test]
        public void GetValue_WhenDoubleValue_ReturnValueFromDoubleParser()
        {
            //Arrange
            var inputValue = "42.88";
            var expectedOutputValue = 42.88;
            var doubleParserMock = new Mock<IDoubleParser>();
            var parserService = CreateParserService(doubleParserMock: doubleParserMock);
            doubleParserMock.Setup(mock => mock.Parse(inputValue))
                .Returns(expectedOutputValue);

            //Act
            var returnValue = parserService.GetValue(inputValue, typeof(double));

            //Assert
            Assert.AreEqual(expectedOutputValue, returnValue);
            doubleParserMock.Verify(mock => mock.Parse(inputValue), Times.Once);
            doubleParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenDateTimeValue_ReturnValueFromDateTimeParser()
        {
            //Arrange
            var expectedOutputValue = new DateTime(2020, 6, 4);
            var inputValue = expectedOutputValue.ToShortDateString();
            var dateTimeParserMock = new Mock<IDateTimeParser>();
            var parserService = CreateParserService(dateTimeParserMock: dateTimeParserMock);
            dateTimeParserMock.Setup(mock => mock.Parse(inputValue, It.IsAny<string[]>()))
                .Returns(expectedOutputValue);

            //Act
            var returnValue = parserService.GetValue(inputValue, typeof(DateTime));

            //Assert
            Assert.AreEqual(expectedOutputValue, returnValue);
            dateTimeParserMock.Verify(mock => mock.Parse(inputValue, It.IsAny<string[]>()), Times.Once);
            dateTimeParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanStringValue_ReturnValueFromLookupParser()
        {
            //Arrange
            var expectedOutputValue = false;
            var inputValue = "false";
            var booleanParserMock = new Mock<IBooleanParser>();
            var parserService = CreateParserService(booleanParserMock: booleanParserMock);
            booleanParserMock.Setup(mock => mock.Parse(inputValue))
                .Returns(expectedOutputValue);

            //Act
            var returnValue = parserService.GetValue(inputValue, typeof(bool));

            //Assert
            Assert.AreEqual(expectedOutputValue, returnValue);
            booleanParserMock.Verify(mock => mock.Parse(inputValue), Times.Once);
            booleanParserMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetValue_WhenBooleanParserThrowsException_ThrowException()
        {
            //Arrange
            var inputValue = double.MaxValue;
            var booleanParserMock = new Mock<IBooleanParser>();
            var parserService = CreateParserService(booleanParserMock: booleanParserMock);
            booleanParserMock.Setup(mock => mock.Parse(inputValue))
                .Throws(ParserException.CreateUnsupportedValueException(inputValue, "boolean"));

            //Act
            Assert.Throws<ParserException>(() => parserService.GetValue(inputValue, typeof(bool)));
        }

        [Test]
        public void GetValue_WhenNotSupportedExceptionType_ThrowException()
        {
            //Arrange
            var inputValue = int.MaxValue;
            var parserService = CreateParserService();

            //Act
            Assert.Throws<UnsupportedDataTypeParserException>(() => parserService.GetValue(inputValue, typeof(Movie)));
        }

        [TestCase("TestValue1", TestEnum.TestValue1)]
        [TestCase("Value1", TestEnum.TestValue1)]
        public void GetValue_WhenEnumStringValue_ReturnValueFromLookupParser(string inputValue, TestEnum expectedOutputValue)
        {
            //Arrange
            var enumParserMock = new Mock<IEnumParser>();
            var parserService = CreateParserService(enumParserMock: enumParserMock);
            enumParserMock.Setup(mock => mock.Parse(inputValue, typeof(TestEnum)))
                .Returns(expectedOutputValue);

            //Act
            var returnValue = parserService.GetValue(inputValue, typeof(TestEnum));

            //Assert
            Assert.AreEqual(expectedOutputValue, returnValue);
            enumParserMock.Verify(mock => mock.Parse(inputValue, typeof(TestEnum)), Times.Once);
            enumParserMock.VerifyNoOtherCalls();
        }
    }
}
