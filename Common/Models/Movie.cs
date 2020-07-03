using Common.Attributes;
using Common.Enums;
using System;

namespace Common.Models
{
    public class Movie
    {
        [ExcelImportDetail("A")]
        [ExcelExportDetail("A", null)]
        public bool? IsAdult { get; set; }
        [ExcelImportDetail("B")]
        [ExcelExportDetail("B")]
        public string? Genre { get; set; }
        [ExcelImportDetail("C")]
        [ExcelExportDetail("C", "Identifier")]
        public string? ImdbId { get; set; }
        [ExcelImportDetail("D")]
        [ExcelExportDetail("D")]
        public string? Overview { get; set; }
        [ExcelImportDetail("E")]
        [ExcelExportDetail("E")]
        public double? Popularity { get; set; }
        [ExcelImportDetail("F")]
        [ExcelExportDetail("F")]
        public DateTime ReleaseDate { get; set; }
        [ExcelImportDetail("G")]
        [ExcelExportDetail("G")]
        public double? Revenue { get; set; }
        [ExcelImportDetail("H")]
        [ExcelExportDetail("H")]
        public double? Runtime { get; set; }
        [ExcelImportDetail("I")]
        [ExcelExportDetail("I")]
        public string? TagLine { get; set; }
        [ExcelImportDetail("J")]
        [ExcelExportDetail("J", null, "#93117E")]
        public string? Title { get; set; }
        [ExcelImportDetail("K")]
        [ExcelExportDetail("K", null, null, true)]
        public double? VoteAverage { get; set; }

        public Rating Rating { get; set; }
    }
}
