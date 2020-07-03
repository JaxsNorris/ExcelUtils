using Analyser.Extensions;
using ConsoleApp.Runners;
using Exporter.Extensions;
using Generator.Extensions;
using Importer.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                           .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}")
                           .WriteTo.File("log.txt", outputTemplate: "{Message:lj}{NewLine}")
                           .CreateLogger();

                var serviceProvider = CreateServiceProvider();

                Log.Information("Running App");
                var app = serviceProvider.GetService<App>();
                await app.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Something broke \n{ex.Message}");
            }
            Log.Information("Press enter to complete");
            Console.ReadLine();
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddSerilog());
            serviceCollection.AddSingleton<App>();
            serviceCollection.AddSingleton<GeneratorRunner>();

            serviceCollection.RegisterImporterDependencies();
            serviceCollection.RegisterExporterDependencies();
            serviceCollection.RegisterAnalyserDependencies();
            serviceCollection.RegisterGeneratorDependencies();
            return serviceCollection.BuildServiceProvider();
        }
    }
}
