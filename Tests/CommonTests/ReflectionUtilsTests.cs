
using Common.Attributes;
using Common.Utils;
using CommonTests.Enums;
using CommonTests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CommonTests
{
    public class ReflectionUtilsTests
    {
        [Test]
        public void CreateLookupDictionary_WhenNoAttributes_ReturnEmptyDictionary()
        {
            //Act
            var lookupDictionary = typeof(TestWithoutAttributes).CreateLookupDictionary<ExcelExportDetailAttribute>();

            //Assert
            Assert.NotNull(lookupDictionary);
            Assert.Zero(lookupDictionary.Count);
        }

        [Test]
        public void CreateLookupDictionary_WhenAttributes_ReturnPopulatedDictionary()
        {
            //Act
            var lookupDictionary = typeof(TestWithAttributes).CreateLookupDictionary<ExcelExportDetailAttribute>();

            //Assert
            Assert.NotNull(lookupDictionary);
            Assert.AreEqual(4, lookupDictionary.Count, $"Should exclude the {nameof(TestWithAttributes.NoAttributeProperty)} as it doesn't have an attribute");

            foreach (var item in lookupDictionary)
            {
                switch (item.Value.ColumnAddress)
                {
                    case "A":
                        AssertLookupValue(item, nameof(TestWithAttributes.StringTest), "ColumnA");
                        break;
                    case "B":
                        AssertLookupValue(item, nameof(TestWithAttributes.DoubleTest), "ColumnB");
                        break;
                    case "C":
                        AssertLookupValue(item, nameof(TestWithAttributes.BooleanTest), "ColumnC");
                        break;
                    case "D":
                        AssertLookupValue(item, nameof(TestWithAttributes.DateTimeTest), "ColumnD");
                        break;
                    default:
                        Assert.True(false, "Only expected columns [A,B,C,D]");
                        break;
                }
            }
        }

        private static void AssertLookupValue(System.Collections.Generic.KeyValuePair<System.Reflection.PropertyInfo, ExcelExportDetailAttribute> item, string expectedPropertyName, string expectedColumnHeader)
        {
            Assert.AreEqual(expectedPropertyName, item.Key.Name);
            Assert.AreEqual(expectedColumnHeader, item.Value.ColumnHeader);
        }

        [Test]
        public void CreateEnumLookupDictionary_WhenTypeNotEnum_ThrowArguementException()
        {
            //Act
            var exception = Assert.Throws<ArgumentException>(() => typeof(TestWithoutAttributes).CreateEnumLookupDictionary());

            //Assert
            Assert.True(exception.Message.Contains("Type provided must be an Enum"));
        }

        [Test]
        public void CreateEnumLookupDictionary_WhenNoAttributes_ReturnEmptyDictionary()
        {
            //Act
            var lookupDictionary = typeof(TestEnumWithoutAttributes).CreateEnumLookupDictionary();

            //Assert
            Assert.NotNull(lookupDictionary);
            Assert.Zero(lookupDictionary.Count);
        }

        [Test]
        public void CreateEnumLookupDictionary_WhenAttributes_ReturnPopulatedDictionary()
        {
            //Act
            var lookupDictionary = typeof(TestEnumWithAttributes).CreateEnumLookupDictionary();

            //Assert
            Assert.NotNull(lookupDictionary);
            Assert.AreEqual(3, lookupDictionary.Count, $"Should exclude the {nameof(TestEnumWithAttributes.None)} enum value as it doesn't have an attribute");
            AssertEnumLookup(lookupDictionary, TestEnumWithAttributes.TestValue1, "One");
            AssertEnumLookup(lookupDictionary, TestEnumWithAttributes.TestValue2, "Value2", "Two");
            AssertEnumLookup(lookupDictionary, TestEnumWithAttributes.TestValue3, "Value3", "Three", "Another");
        }

        private static void AssertEnumLookup(IReadOnlyDictionary<object, string[]> lookupDictionary, TestEnumWithAttributes key, params string[] expectedLookupValues)
        {
            var lookupValue = lookupDictionary.GetValueOrDefault(key);
            Assert.NotNull(lookupValue);
            Assert.AreEqual(expectedLookupValues.Length, lookupValue.Length);
            Assert.AreEqual(expectedLookupValues, lookupValue);
        }
    }
}
