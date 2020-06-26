using Common.Interfaces.Parsers;
using Importer.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace Importer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterImporterDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton<IDoubleParser, DoubleParser>();
            collection.AddSingleton<IDateTimeParser, DateTimeParser>();
            collection.AddSingleton<ILookupParser, LookupParser>();
            collection.AddSingleton<IParserService, ParserService>();
            return collection;
        }
    }
}
