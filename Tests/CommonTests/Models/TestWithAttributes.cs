using Common.Attributes;
using System;

namespace CommonTests.Models
{
    internal class TestWithAttributes
    {
        [ExcelImportDetail("A")]
        [ExcelExportDetail("A", "ColumnA")]
        public string StringTest { get; set; }
        [ExcelImportDetail("B")]
        [ExcelExportDetail("B", "ColumnB")]
        public double DoubleTest { get; set; }
        [ExcelImportDetail("C")]
        [ExcelExportDetail("C", "ColumnC")]
        public bool BooleanTest { get; set; }
        [ExcelImportDetail("D")]
        [ExcelExportDetail("D", "ColumnD")]
        public DateTime DateTimeTest { get; set; }

        public string NoAttributeProperty { get; set; }
    }
}
