using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Exporter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterExporterDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton(typeof(IExporter<>), typeof(ExcelExporter<>));
            return collection;
        }
    }
}
