using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace CodingTracker;

public static class Utils
{
    private const string AppSettingsFileName = "appsettings.json";
    private static readonly string AppSettingsPath = FindDirectoryOfFile(
        Directory.GetCurrentDirectory(), AppSettingsFileName) + "/" + AppSettingsFileName;
    public static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .AddJsonFile(AppSettingsPath)
        .AddEnvironmentVariables()
        .Build();

    public static string? FindDirectoryOfFile(string startingDirectory, string fileName)
    {
        string currentDirectory = startingDirectory;

        while (true)
        {
            string filePath = Path.Combine(currentDirectory, fileName);
            if (File.Exists(filePath))
            {
                return currentDirectory;
            }

            // Move to the parent directory
            var parentDirectory = Directory.GetParent(currentDirectory);
            if (parentDirectory == null)
            {
                // Reached the root of the filesystem
                break;
            }
            currentDirectory = parentDirectory.FullName;
        }

        // File not found
        return null;
    }
}