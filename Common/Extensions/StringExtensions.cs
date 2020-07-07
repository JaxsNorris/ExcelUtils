﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitCamelCase(this string camelCaseString)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(camelCaseString, pattern);
            return matches.Select(match => match.Value.ToLower());
        }

        public static string FromCamelCase(this string camelCaseString)
        {
            var splits = camelCaseString.SplitCamelCase();
            var returnValue = string.Join(" ", splits);
            return returnValue.CapitalizedFirstLetter();
        }

        public static string CapitalizedFirstLetter(this string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        public static string GetValueOrDefault(this string? value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}
