
using Common.Attributes;
using System;

namespace Common
{
    public class Movie
    {
        [ExcelImportDetail("A")]
        [ExcelExportDetailAttribute("A", null)]
        public bool? IsAdult { get; set; }
        [ExcelImportDetail("B")]
        [ExcelExportDetailAttribute("B")]
        public string? Genre { get; set; }
        [ExcelImportDetail("C")]
        [ExcelExportDetailAttribute("C", "Identifier")]
        public string? ImdbId { get; set; }
        [ExcelImportDetail("D")]
        [ExcelExportDetailAttribute("D")]
        public string? Overview { get; set; }
        [ExcelImportDetail("E")]
        [ExcelExportDetailAttribute("E")]
        public double? Popularity { get; set; }
        [ExcelImportDetail("F")]
        [ExcelExportDetailAttribute("F")]
        public DateTime ReleaseDate { get; set; }
        [ExcelImportDetail("G")]
        [ExcelExportDetailAttribute("G")]
        public double? Revenue { get; set; }
        [ExcelImportDetail("H")]
        [ExcelExportDetailAttribute("H")]
        public double? Runtime { get; set; }
        [ExcelImportDetail("I")]
        [ExcelExportDetailAttribute("I")]
        public string? TagLine { get; set; }
        [ExcelImportDetail("J")]
        [ExcelExportDetailAttribute("J", null, "#93117E")]
        public string? Title { get; set; }
        [ExcelImportDetail("K")]
        [ExcelExportDetailAttribute("K", null, null, true)]
        public double? VoteAverage { get; set; }
    }
}
