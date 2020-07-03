using System.Runtime.Serialization;

namespace Common.Extensions
{
    public static class SerializationInfoExtensions
    {
        public static string GetStringOrEmpty(this SerializationInfo info, string name)
        {
            var value = info.GetString(name);
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value;
        }
    }
}
