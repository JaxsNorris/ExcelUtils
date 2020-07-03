using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Reader.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterReaderDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton<IReaderFactory, ExcelReaderFactory>();
            return collection;
        }
    }
}
