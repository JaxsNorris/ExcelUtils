using System.Collections.Generic;

namespace Importer
{
    public static class ParserConstants
    {
        public static readonly IReadOnlyDictionary<bool, string[]> BoolDefaultMapping = new Dictionary<bool, string[]>() {
                { false,  new string[]{ "0","off", "no", "false" } },
                { true,  new string[]{ "1","on", "yes", "true" } }
            };
    }
}
