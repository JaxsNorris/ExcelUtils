using Common.Extensions;
using NUnit.Framework;

namespace CommonTests
{
    public class StringExtensionsTests
    {
        private const string DefaultValue = "DefaultValue";

        [TestCase("aPerfectWorld", "a", "perfect", "world")]
        [TestCase("theAnswerIs42", "the", "answer", "is", "42")]
        public void SplitCamelCase_GiveCamelCase_ReturnSplits(string input, params string[] expectedOutput)
        {
            //Act
            var output = input.SplitCamelCase();

            //Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [TestCase("aPerfectWorld", "A perfect world")]
        [TestCase("theAnswerIs42", "The answer is 42")]
        public void FromCamelCase_GiveCamelCase_ReturnSplitsString(string input, string expectedOutput)
        {
            //Act
            var output = input.FromCamelCase();

            //Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [TestCase("aPerfectWorld", "APerfectWorld")]
        [TestCase("a Perfect World", "A Perfect World")]
        [TestCase("a perfect world", "A perfect world")]
        public void CapitalizedFirstLetter_GivenInput_ReturnCorrectOutput(string input, string expectedOutput)
        {
            //Act
            var output = input.CapitalizedFirstLetter();

            //Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [TestCase("", DefaultValue)]
        [TestCase(" ", DefaultValue)]
        [TestCase(null, DefaultValue)]
        [TestCase("A perfect world", "A perfect world")]
        public void GetValueOrDefault_GiveInput_ReturnCorrectValue(string input, string expectedOutput)
        {
            //Act
            var output = input.GetValueOrDefault(DefaultValue);

            //Assert
            Assert.AreEqual(expectedOutput, output);
        }
    }
}
