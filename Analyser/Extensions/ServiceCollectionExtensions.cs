using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Analyser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAnalyserDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton<IAnalyser, ExcelAnalyser>();
            return collection;
        }
    }
}
