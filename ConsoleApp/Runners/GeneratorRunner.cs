using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using ConsoleApp.Utils;
using Importer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp.Runners
{
    public class GeneratorRunner
    {
        private readonly IDataGenerator _dataGenerator;
        private readonly List<ColumnDataGenerationOption> _columnSetup;

        public GeneratorRunner(IDataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
            _columnSetup = CreateDefaultColumnSetup();
        }

        public async Task Run()
        {
            await GenerateData("Small", 5000);
            await GenerateData("Medium", 30000);
            await GenerateData("Big", 500000);
            Console.WriteLine("Finished saving file");
        }

        private async Task GenerateData(string keyword, int numberOfItems)
        {
            var fullpath = FileUtils.GetPath(Constants.OutputDirectory, $"{keyword}{Constants.DefaultFilename}");
            if (UserInputUtil.GetUserBoolInput($"Generate {keyword} data set?"))
            {
                Console.WriteLine($"Generating {keyword} worksheet....");
                await _dataGenerator.GenerateData(fullpath, "data", numberOfItems, _columnSetup);
            }
        }

        private List<ColumnDataGenerationOption> CreateDefaultColumnSetup()
        {
            return new List<ColumnDataGenerationOption>()
            {
                new ColumnDataGenerationOption(){ CoulmnHeader = "Blank column", ColumnFieldType = FieldTypes.None, HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text only", ColumnFieldType = FieldTypes.Text, HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Bool only", ColumnFieldType = FieldTypes.Boolean},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Number only", ColumnFieldType = FieldTypes.Number,HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Datetime only", ColumnFieldType = FieldTypes.DateTime},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Lookup values ", ColumnFieldType = FieldTypes.Text, Values = GetLookupValues(10)},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Boolean Lookup values ", ColumnFieldType = FieldTypes.Text, Values = GetBooleanLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Rating Lookup values ", ColumnFieldType = FieldTypes.Text, Values = GetRatingLookupValues(),HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "All", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.Number, FieldTypes.Boolean ,FieldTypes.DateTime},HasBlankRows = true },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text Number", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.Number } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text Bool", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.Boolean } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text Date", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.DateTime } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text Number bool", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.Number, FieldTypes.Boolean } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Text Number Date", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Text,FieldTypes.Number, FieldTypes.DateTime } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Number Bool", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Number,FieldTypes.Boolean } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Number Date", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Number,FieldTypes.DateTime } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Number Date Bool", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Number,FieldTypes.DateTime, FieldTypes.Boolean } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Date Bool", ColumnFieldType = FieldTypes.Text, OtherFieldTypes = new FieldTypes[]{FieldTypes.Boolean,FieldTypes.DateTime } },
            };
        }

        private string[] GetLookupValues(int number)
        {
            var items = new string[number];
            for (var i = 0; i < number; i++)
            {
                items[i] = $"Value {i}";
            }
            return items;
        }

        private string[] GetBooleanLookupValues()
        {
            var items = new List<string>();
            foreach (var mappings in ParserConstants.BoolDefaultMapping)
            {
                items.AddRange(mappings.Value);
            }
            return items.ToArray();
        }

        private string[] GetRatingLookupValues()
        {
            var items = new List<string>();
            foreach (var mappings in ParserConstants.RatingDefaultMapping)
            {
                items.AddRange(mappings.Value);
            }
            return items.ToArray();
        }
    }
}
