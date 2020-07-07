
using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumLookupAttribute : Attribute
    {
        public string[] LookupDictionaryValues { get; private set; }

        public EnumLookupAttribute(params string[] lookupDictionaryValue)
        {
            LookupDictionaryValues = lookupDictionaryValue;
        }
    }
}
