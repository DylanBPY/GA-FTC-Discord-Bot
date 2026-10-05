using System;
using System.IO;

namespace Util;

class EnvLoader
{
    public static void Load()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (!File.Exists(filePath)) return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }

            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            // Store as environment variable
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}