using Common.Enums;
using System.Collections.Generic;

namespace Importer
{
    public static class ParserConstants
    {
        public static readonly IReadOnlyDictionary<bool, string[]> BoolDefaultMapping = new Dictionary<bool, string[]>() {
                { false,  new string[]{ "0","off", "no", "false" } },
                { true,  new string[]{ "1","on", "yes", "true" } }
            };

        public static readonly IReadOnlyDictionary<Rating, string[]> RatingDefaultMapping = new Dictionary<Rating, string[]>() {
                { Rating.OneStar,  new string[]{ "1","terrible", "1 star","*" } },
                { Rating.TwoStar,  new string[]{ "2","not good", "2 stars", "2 star", "**"} },
                { Rating.ThreeStar,  new string[]{ "3","ok", "3 stars", "3 star", "***"} },
                { Rating.FourStar,  new string[]{ "4","pretty good", "4 stars" , "4 star", "****" } },
                { Rating.FiveStar,  new string[]{ "5","wow", "5 stars", "5 star", "*****"} },
            };
    }
}
