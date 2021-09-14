using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Common.Utils;
using ConsoleApp.Utils;
using Importer;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _columnSetup = CreateMemberDataColumnSetup();
        }

        public async Task Run()
        {
            await GenerateData("Tiny", 5);
            /*
             * await GenerateData("Small", 5000);
            await GenerateData("Medium", 30000);
            await GenerateData("Big", 500000);
            */
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

        private List<ColumnDataGenerationOption> CreateMemberDataColumnSetup()
        {
            return new List<ColumnDataGenerationOption>()
            {
                new ColumnDataGenerationOption(){ CoulmnHeader = "Fullname", ColumnFieldType = FieldTypes.Text, HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "cat", ColumnFieldType = FieldTypes.Text, Values =new[] { "cat1", "cat2" } },
                new ColumnDataGenerationOption(){ CoulmnHeader = "Gender", ColumnFieldType = FieldTypes.Text, Values = GetGenderLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Date of birth", ColumnFieldType = FieldTypes.DateTime},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Salary", ColumnFieldType = FieldTypes.Number, HasBlankRows = true},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Expat", ColumnFieldType = FieldTypes.Text, Values = GetBooleanLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Disabled", ColumnFieldType = FieldTypes.Text, Values = GetBooleanLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Date of disability", ColumnFieldType = FieldTypes.DateTime},
                new ColumnDataGenerationOption(){ CoulmnHeader = "country", ColumnFieldType = FieldTypes.Text, Values = GetLocationLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Region", ColumnFieldType = FieldTypes.Text, Values = GetRegionLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "Industry", ColumnFieldType = FieldTypes.Text, Values = GetIndustyLookupValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "CTA", ColumnFieldType = FieldTypes.Text, Values= GetMinMaxValues()},
                new ColumnDataGenerationOption(){ CoulmnHeader = "multiple", ColumnFieldType = FieldTypes.Number, Values= GetMinMaxValues(2,5)},
                new ColumnDataGenerationOption(){ CoulmnHeader = "pec", ColumnFieldType = FieldTypes.Number, Values = new []{ "0.1", "0.2", "0.5" } },
            };
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

        private string[] GetMinMaxValues(int start = 60, int end = 70)
        {
            return Enumerable.Range(start, end).Select(x => x.ToString()).ToArray();
        }

        private string[] GetLocationLookupValues()
        {
            return new[] {
                "ZAF",
                "KEN",
                "LSO",
            };
        }

        private string[] GetRegionLookupValues()
        {
            return new[] {
                "EC",
                "FS",
                "GP",
                "KZN",
                "LP",
                "MP",
                "NW",
                "NC",
                "WC",
            };
        }

        private string[] GetIndustyLookupValues()
        {
            return new[] {
                "Professional",
                "Administration",
                "Retail & Wholesale Companies",
                "D",
                "E",
                "F",
                "G"
            };
        }

        private string[] GetGenderLookupValues()
        {
            return new[] { "F", "M" };
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
