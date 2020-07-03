using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Generator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterGeneratorDependencies(this IServiceCollection collection)
        {
            collection.AddSingleton<IDataGenerator, DataGenerator>();
            return collection;
        }
    }
}
