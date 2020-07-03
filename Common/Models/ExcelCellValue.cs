namespace Common.Models
{
    public class ExcelCellValue
    {
        public string Address { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

        public object? Value { get; set; }
        public string? TextValue { get; set; }

        public ExcelCellValue(string address, int row, int column, object? value, string? textValue)
        {
            Address = address;
            Column = column;
            Row = row;
            Value = value;
            TextValue = textValue;
        }
    }
}
