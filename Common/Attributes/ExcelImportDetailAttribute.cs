using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelImportDetailAttribute : Attribute
    {
        public string Column { private set; get; }

        public ExcelImportDetailAttribute(string column)
        {
            Column = column;
        }
    }
}
