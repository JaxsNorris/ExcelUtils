using System;
using System.IO;

namespace Common.Utils
{
    public static class FileUtils
    {
        public static string GetPath(string directory, string filename, string? subDirectory = null)
        {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(rootDir))
                throw new InvalidOperationException("No rootDir found");

            string path;
            if (string.IsNullOrWhiteSpace(subDirectory))
                path = Path.Combine(rootDir, directory);
            else
                path = Path.Combine(rootDir, directory, subDirectory);

            Directory.CreateDirectory(path);
            return Path.Combine(path, filename);
        }
    }
}
