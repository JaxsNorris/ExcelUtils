
using Common.Exceptions;
using Importer.Parsers;
using NUnit.Framework;
using System;
using Tests.Common.TestClasses;

namespace ImporterTests.Parsers
{
    public class EnumParserTests
    {
        private EnumParser CreateEnumParser()
        {
            return new EnumParser();
        }

        [Test]
        public void Parse_WhenNull_ReturnNullValue()
        {
            //Arrange
            var parser = CreateEnumParser();

            //Act
            var parsedValue = parser.Parse(null, typeof(TestEnum));

            //Assert
            Assert.Null(parsedValue);
        }

        [Test]
        public void Parse_WhenNotSupportedType_ThrowException()
        {
            //Arrange
            var parser = CreateEnumParser();
            var invalidTypeValue = DateTime.MinValue;

            //Act
            var exception = Assert.Throws<UnsupportedDataTypeParserException>(() => parser.Parse(invalidTypeValue, typeof(TestEnum)));

            //Assert
            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains(invalidTypeValue.GetType().Name));
        }

        [Test]
        public void Parse_WhenStringValueNotContainedInAttribute_ThrowException()
        {
            //Arrange
            var parser = CreateEnumParser();

            //Act
            var exception = Assert.Throws<ParserException>(() => parser.Parse("! in Lookup", typeof(TestEnum)));

            //Assert
            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains("Lookup dictionary", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestCase(TestEnum.TestValue1)]
        [TestCase(TestEnum.TestValue2)]
        [TestCase(TestEnum.TestValue3)]
        public void Parse_WhenEnumValue_ReturnBoolValue(TestEnum value)
        {
            //Arrange
            var parser = CreateEnumParser();

            //Act
            var parsedValue = parser.Parse(value, typeof(TestEnum));

            //Assert
            Assert.AreEqual(value, parsedValue);
        }

        [TestCase("Value1", TestEnum.TestValue1)]
        [TestCase("Two", TestEnum.TestValue2)]
        [TestCase("3", TestEnum.TestValue3)]
        public void Parse_WhenEnumLookupValue_ReturnBoolValue(string value, TestEnum expectedParsedValue)
        {
            //Arrange
            var parser = CreateEnumParser();

            //Act
            var parsedValue = parser.Parse(value, typeof(TestEnum));

            //Assert
            Assert.AreEqual(expectedParsedValue, parsedValue);
        }
    }
}
