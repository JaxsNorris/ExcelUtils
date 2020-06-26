using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelExportDetailAttribute : Attribute
    {
        public string Column { private set; get; }
        public string? ColumnHeader { private set; get; }
        public string? ColumnColorHex { private set; get; }
        public bool IsHidden { private set; get; }

        public ExcelExportDetailAttribute(string column, string? columnHeader = null, string? columnColorHex = null, bool isHidden = false)
        {
            Column = column;
            ColumnHeader = columnHeader;
            ColumnColorHex = columnColorHex;
            IsHidden = isHidden;
        }
    }
}
