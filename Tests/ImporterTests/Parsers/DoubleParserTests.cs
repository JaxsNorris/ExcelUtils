using Common.Exceptions;
using Importer.Parsers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;

namespace ImporterTests.Parsers
{
    public class DoubleParserTests
    {
        private DoubleParser CreateDoubleParser()
        {
            return new DoubleParser();
        }

        [Test]
        public void Parse_WhenNull_ReturnNullValue()
        {
            var parser = CreateDoubleParser();

            var parsedValue = parser.Parse(null);

            Assert.Null(parsedValue);
        }

        [TestCase(42)]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void Parse_WhenDouble_ReturnDoubleValue(double doubleValue)
        {
            var parser = CreateDoubleParser();

            var parsedValue = parser.Parse(doubleValue);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(doubleValue, parsedValue.Value);
        }

        [TestCase(true)]
        [TestCase('t')]
        public void Parse_WhenNotSupportedType_ThrowParserException(object invalidTypeValue)
        {
            var parser = CreateDoubleParser();

            var exception = Assert.Throws<UnsupportedDataTypeParserException>(() => parser.Parse(invalidTypeValue));

            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains(invalidTypeValue.GetType().Name));
        }

        [TestCase(short.MinValue, short.MinValue)]
        [TestCase(short.MaxValue, short.MaxValue)]
        [TestCase(int.MinValue, int.MinValue)]
        [TestCase(int.MaxValue, int.MaxValue)]
        [TestCase(long.MinValue, long.MinValue)]
        [TestCase(long.MaxValue, long.MaxValue)]
        [TestCase(float.MinValue, float.MinValue)]
        [TestCase(float.MaxValue, float.MaxValue)]
        public void Parse_WhenNumericType_ReturnDoubleValue(object value, double expectedValue)
        {
            var parser = CreateDoubleParser();

            var parsedValue = parser.Parse(value);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(expectedValue, parsedValue.Value);
        }

        private static readonly IEnumerable<decimal> decimalTestValues = new List<decimal>
            {
                decimal.MaxValue,
                decimal.MinValue,
            };
        [TestCaseSource(nameof(decimalTestValues))]
        public void Parse_WhenDecimalValue_ReturnDoubleValue(decimal value)
        {
            var parser = CreateDoubleParser();

            var parsedValue = parser.Parse(value);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(value, parsedValue.Value);
        }

        [TestCase("-1035,77219", -1035.77219)]
        [TestCase("1e-35", 1E-35)]
        [TestCase("1635592999999999999999999", 1.635593E+24)]
        [TestCase("-17,455", -17.455)]
        [TestCase("190,34001", 190.34001)]
        public void Parse_WhenStringValue_ReturnDoubleValue(string value, double expectedValue)
        {
            var parser = CreateDoubleParser();

            var parsedValue = parser.Parse(value);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(expectedValue, parsedValue.Value);
        }

        [TestCase("-1,035.77219")]
        [TestCase("1,635,592,999,999,999,999,999,999")]
        [TestCase("1AFF")]
        public void Parse_WhenStringValueWithDifferentCulture_ThrowParserException(string differentCultureDouble)
        {
            var parser = CreateDoubleParser();

            var exception = Assert.Throws<ParserException>(() => parser.Parse(differentCultureDouble));

            Assert.NotNull(exception.InnerException);
            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains("Input string was not in a correct format"));
        }

        [TestCase("-1,035.77219", -1035.77219)]
        [TestCase("1e-35", 1E-35)]
        [TestCase("1,635,592,999,999,999,999,999,999", 1.635593E+24)]
        [TestCase("-17.455", -17.455)]
        [TestCase("190.34001", 190.34001)]
        public void Parse_WhenStringValueWithDifferentCulture_ReturnDoubleValue(string differentCultureDouble, double expectedValue)
        {
            var parser = CreateDoubleParser();
            var culture = new CultureInfo("en-US");

            var parsedValue = parser.Parse(differentCultureDouble, culture);

            Assert.NotNull(parsedValue);
            Assert.AreEqual(expectedValue, parsedValue.Value);
        }

        [Test]
        public void Parse_WhenStringValueWithCultureAndInvalidString_ThrowParserException()
        {
            var parser = CreateDoubleParser();
            var culture = new CultureInfo("en-US");

            var exception = Assert.Throws<ParserException>(() => parser.Parse("1AFF", culture));

            Assert.NotNull(exception.InnerException);
            Assert.NotNull(exception.UiErrorMessage);
            Assert.IsTrue(exception.Message.Contains("Input string was not in a correct format"));
        }
    }
}
