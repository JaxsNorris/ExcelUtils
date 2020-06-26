using Common;
using Importer;
using Importer.Parsers;
using NUnit.Framework;
using System.Linq;
using Tests.Common;

namespace ImporterTests
{
    public class FileImporterTests
    {
        private FileImporter<Movie> GetFileImporter()
        {
            var doubleParser = new DoubleParser();
            var parserService = new ParserService(new DateTimeParser(doubleParser), doubleParser, new LookupParser());
            return new FileImporter<Movie>(parserService);
        }

        [Test]
        public void Import_When_ValidData_FetchAllMovies()
        {
            var importer = GetFileImporter();
            var expectedResults = DataGenerator.GetMovieList();
            var path = ResourceFileHelper.GetPath("ExportedMovieData.xlsx");
            var sheetname = $"Test-06-26-20 13_39_13";
            var results = importer.Import(path, sheetname, 2, 2);

            Assert.AreEqual(expectedResults.Count, results.Count());

            foreach (var expected in expectedResults)
            {
                var actual = results.SingleOrDefault(x => expected.Title.Equals(x.Title, System.StringComparison.InvariantCulture));
                AssertMovie(expected, actual);
            }
        }

        private void AssertMovie(Movie expected, Movie actual)
        {
            Assert.NotNull(actual);
            Assert.AreEqual(expected.IsAdult, actual.IsAdult);
            Assert.AreEqual(expected.ImdbId, actual.ImdbId);
            Assert.NotNull(actual.Overview);
            Assert.AreEqual(expected.Popularity, actual.Popularity);
            Assert.AreEqual(expected.ReleaseDate, actual.ReleaseDate);
            Assert.AreEqual(expected.Runtime, actual.Runtime);
            Assert.AreEqual(expected.Revenue, actual.Revenue);
        }
    }
}
