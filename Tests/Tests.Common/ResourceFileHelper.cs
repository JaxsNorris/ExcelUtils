using System;
using System.IO;

namespace Tests.Common
{
    public static class ResourceFileHelper
    {
        public static FileInfo GetFileInfo(string file)
        {
            return new FileInfo(GetPath(file));
        }

        public static string GetPath(string file, string subDirectory = null)
        {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            string path;
            if (string.IsNullOrWhiteSpace(subDirectory))
                path = Path.Combine(rootDir, "Resources");
            else
                path = Path.Combine(rootDir, "Resources", subDirectory);

            Directory.CreateDirectory(path);
            return Path.Combine(path, file);
        }
    }
}
