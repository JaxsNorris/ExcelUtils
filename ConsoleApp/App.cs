using ConsoleApp.Runners;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class App
    {
        private readonly GeneratorRunner _generatorRunner;

        public App(GeneratorRunner generatorRunner)
        {
            _generatorRunner = generatorRunner;
        }

        public async Task Run()
        {
            await _generatorRunner.Run();

        }
    }
}
