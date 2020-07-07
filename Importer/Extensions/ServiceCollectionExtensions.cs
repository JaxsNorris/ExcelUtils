using Common.Interfaces;
using Common.Interfaces.Parsers;
using Importer.Parsers;
using Importer.Reader;
using Microsoft.Extensions.DependencyInjection;

namespace Importer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterImporterDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton(typeof(IImporter<>), typeof(ExcelImporter<>));
            collection.AddSingleton<IDoubleParser, DoubleParser>();
            collection.AddSingleton<IDateTimeParser, DateTimeParser>();
            collection.AddSingleton<ILookupParser, LookupParser>();
            collection.AddSingleton<IBooleanParser, BooleanParser>();
            collection.AddSingleton<IEnumParser, EnumParser>();
            collection.AddSingleton<IParserService, ParserService>();
            collection.AddSingleton<IReaderFactory, ExcelReaderFactory>();
            return collection;
        }
    }
}
