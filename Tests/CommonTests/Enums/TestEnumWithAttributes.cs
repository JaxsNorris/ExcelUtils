using Common.Attributes;

namespace CommonTests.Enums
{
    public enum TestEnumWithAttributes
    {
        None = 0,
        [EnumLookup("One")]
        TestValue1 = 1,
        [EnumLookup("Value2", "Two")]
        TestValue2 = 2,
        [EnumLookup("Value3", "Three", "Another")]
        TestValue3 = 3
    }
}
