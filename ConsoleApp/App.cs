using Common.Interfaces.Parsers;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class App
    {
        private readonly IParserService _parserService;

        public App(IParserService parserService)
        {
            _parserService = parserService;
        }

        public Task Run()
        {
            var dt = _parserService.ParseDateTime("test", "2020/06/26", null);
            Console.WriteLine(dt);
            return Task.CompletedTask;
        }
    }
}
