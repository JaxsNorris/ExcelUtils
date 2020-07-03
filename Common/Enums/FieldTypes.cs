
using System;

namespace Common.Enums
{
    [Flags]
    public enum FieldTypes
    {
        None = 0,
        Text = 2,
        Number = 4,
        Boolean = 8,
        DateTime = 16,
        All = Text | Number | Boolean | DateTime
    }
}
