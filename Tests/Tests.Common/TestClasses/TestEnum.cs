using Common.Attributes;

namespace Tests.Common.TestClasses
{
    public enum TestEnum
    {
        None = 0,
        [EnumLookup("1", "Value1", "One")]
        TestValue1 = 1,
        [EnumLookup("2", "Value2", "Two")]
        TestValue2 = 2,
        [EnumLookup("3", "Value3", "Three")]
        TestValue3 = 3
    }
}
