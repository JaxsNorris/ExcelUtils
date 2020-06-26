using Importer;
using Importer.Parsers;
using NUnit.Framework;
using System.Collections.Generic;

namespace ImporterTests.Parsers
{
    public class LookupParserTests
    {
        private const string DefaultValueOnNull = "defaultValueOnNull";
        private const string DefaultValueOnFailedLookup = "defaultValueOnFailedLookup";

        private const string LookupKey1 = "A-Fruit";
        private const string LookupValue1 = "apple";

        private static readonly IReadOnlyDictionary<string, string[]> TestLookupDictionary = new Dictionary<string, string[]>() {
                { LookupKey1,  new string[]{ LookupValue1, "apricots", "acerola", "avocado" } },
                { "B-Fruit",  new string[]{ "banana", "blackberries", "blueberries" } }
            };

        private LookupParser CreateLookupParser()
        {
            return new LookupParser();
        }

        [Test]
        public void Parse_WhenValueIsNull_ReturnSuppliedDefaultValue()
        {
            var parser = CreateLookupParser();

            var lookupValue = parser.Parse(null, TestLookupDictionary, DefaultValueOnNull, DefaultValueOnFailedLookup);

            Assert.AreEqual(DefaultValueOnNull, lookupValue);
        }

        [Test]
        public void Parse_WhenValueIsNotInLookup_ReturnSuppliedDefaultValue()
        {
            var parser = CreateLookupParser();

            var lookupValue = parser.Parse("Value not in the dictionary", TestLookupDictionary, DefaultValueOnNull, DefaultValueOnFailedLookup);

            Assert.AreEqual(DefaultValueOnFailedLookup, lookupValue);
        }

        [Test]
        public void Parse_WhenValueIsInLookup_ReturnCorrectValue()
        {
            var parser = CreateLookupParser();

            var lookupValue = parser.Parse(LookupValue1, TestLookupDictionary, DefaultValueOnNull, DefaultValueOnFailedLookup);

            Assert.AreEqual(LookupKey1, lookupValue);
        }

        [TestCase("0", false)]
        [TestCase("false", false)]
        [TestCase("true", true)]
        [TestCase("yes", true)]
        public void Parse_WhenBoolLookup_ReturnCorrectValue(string value, bool expected)
        {
            var parser = CreateLookupParser();

            var lookupValue = parser.Parse(value, ParserConstants.BoolDefaultMapping, !expected, !expected);

            Assert.AreEqual(expected, lookupValue);
        }
    }
}
