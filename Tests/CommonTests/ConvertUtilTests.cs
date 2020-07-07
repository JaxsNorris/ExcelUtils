using Common.Utils;
using NUnit.Framework;
using Tests.Common.TestClasses;

namespace CommonTests
{
    public class ConvertUtilTests
    {
        [TestCase(int.MinValue)]
        [TestCase(byte.MinValue)]
        [TestCase(sbyte.MinValue)]
        [TestCase(short.MinValue)]
        [TestCase(ushort.MinValue)]
        [TestCase(int.MinValue)]
        [TestCase(uint.MinValue)]
        [TestCase(double.MinValue)]
        [TestCase(float.MinValue)]
        public void IsNumeric_WhenNumericValue_ReturnTrue(object value)
        {
            //Act
            var isNumeric = value.IsNumeric();

            //Assert
            Assert.IsTrue(isNumeric);
        }

        [TestCase(int.MinValue)]
        [TestCase(byte.MinValue)]
        [TestCase(sbyte.MinValue)]
        [TestCase(short.MinValue)]
        [TestCase(ushort.MinValue)]
        [TestCase(int.MinValue)]
        [TestCase(uint.MinValue)]
        [TestCase(double.MinValue)]
        [TestCase(float.MinValue)]
        public void IsNumeric_WhenNumericType_ReturnTrue(object value)
        {
            //Act
            var isNumeric = value.GetType().IsNumeric();

            //Assert
            Assert.IsTrue(isNumeric);
        }

        [TestCase(TestEnum.TestValue1)]
        [TestCase("not a number")]
        public void IsNumeric_WhenNotNumericValue_ReturnFalse(object value)
        {
            //Act
            var isNumeric = value.IsNumeric();

            //Assert
            Assert.IsFalse(isNumeric);
        }

        [TestCase("123.12", true)]
        [TestCase("123,12", true)]
        [TestCase("number", false)]
        [TestCase("str123number", false)]
        public void IsNumericString_WhenNumericString_ReturnTrue(string value, bool expectedResult)
        {
            //Act
            var isNumeric = value.IsNumericString();

            //Assert
            Assert.AreEqual(expectedResult, isNumeric);
        }
    }
}
