namespace Common.Utils
{
    public static class ExcelHelper
    {
        public static string GetAddress(string column, int row)
        {
            return $"{column}{row}";
        }

        public static string GetFullAddress(string filename, string worksheetName, string address)
        {
            return $"[{filename}]{worksheetName}!{address}";
        }

        public static string GetAddressWithWorksheet(string worksheetName, string address)
        {
            return $"{worksheetName}!{address}";
        }
    }
}
