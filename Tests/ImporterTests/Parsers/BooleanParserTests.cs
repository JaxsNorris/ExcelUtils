using Common.Exceptions;
using Common.Interfaces.Parsers;
using Importer.Parsers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ImporterTests.Parsers
{
    public class BooleanParserTests
    {
        private BooleanParser CreateBooleanParser(IMock<ILookupParser> lookupParserMock = null)
        {
            if (lookupParserMock == null)
                lookupParserMock = new Mock<ILookupParser>();
            return new BooleanParser(lookupParserMock.Object);
        }

        [Test]
        public void Parse_WhenNull_ReturnNullValue()
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(null);

            Assert.Null(parsedValue);
        }

        [Test]
        public void Parse_WhenNotSupportedType_ThrowException()
        {
            var parser = CreateBooleanParser();
            var invalidTypeValue = DateTime.MinValue;

            var exception = Assert.Throws<UnsupportedDataTypeParserException>(() => parser.Parse(invalidTypeValue));

            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains(invalidTypeValue.GetType().Name));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Parse_WhenBooleanValue_ReturnBoolValue(bool value)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(value, parsedValue);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Parse_WhenDoubleValue_ReturnBoolValue(double value, bool expectedResult)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(expectedResult, parsedValue);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Parse_WhenIntValue_ReturnBoolValue(int value, bool expectedResult)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(expectedResult, parsedValue);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Parse_WhenLongValue_ReturnBoolValue(long value, bool expectedResult)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(expectedResult, parsedValue);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Parse_WhenShortValue_ReturnBoolValue(short value, bool expectedResult)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(expectedResult, parsedValue);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        public void Parse_WhenLongValue_ReturnBoolValue(float value, bool expectedResult)
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(value);

            Assert.AreEqual(expectedResult, parsedValue);
        }

        [Test]
        public void Parse_WhenInvalidDoubleValue_ThrowException()
        {
            var parser = CreateBooleanParser();
            var invalidTypeValue = double.MaxValue;

            var exception = Assert.Throws<ParserException>(() => parser.Parse(invalidTypeValue));

            Assert.NotNull(exception.UiErrorMessage);
        }

        [Test]
        public void Parse_WhenEmptyStringValue_ReturnNullValue()
        {
            var parser = CreateBooleanParser();

            var parsedValue = parser.Parse(string.Empty);

            Assert.IsNull(parsedValue);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        public void Parse_WhenLookupParserReturnsValue_ReturnBoolValue(string inputValue, bool returnedValue)
        {
            var lookupParserMock = new Mock<ILookupParser>();
            lookupParserMock.Setup(mock => mock.Parse(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<bool, string[]>>(), null, null))
                                        .Returns(returnedValue);

            var parser = CreateBooleanParser(lookupParserMock);

            var parsedValue = parser.Parse(inputValue);

            Assert.AreEqual(returnedValue, parsedValue);
        }

        [Test]
        public void Parse_WhenInvalidStringValue_ThrowException()
        {
            var parser = CreateBooleanParser();

            var exception = Assert.Throws<ParserException>(() => parser.Parse("This is not a valid bool string"));

            Assert.True(exception.Message.Contains("Unsupported value"));
        }

    }
}
