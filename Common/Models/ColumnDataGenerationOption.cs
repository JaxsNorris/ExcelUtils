using Common.Enums;

namespace Common.Models
{
    public class ColumnDataGenerationOption
    {
        public string CoulmnHeader { get; set; } = string.Empty;
        public FieldTypes ColumnFieldType { get; set; }
        public FieldTypes[]? OtherFieldTypes { get; set; }
        public object[]? Values { get; set; }
        public bool HasBlankRows { get; set; }
    }

}
