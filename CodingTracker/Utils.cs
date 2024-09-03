using Microsoft.Extensions.Configuration;

namespace CodingTracker;

public static class Utils
{
    private const string AppSettingsFileName = "appsettings.json";
    private static readonly string ProjectRootDirectory = GetProjectRootDirectory();
    private static readonly string AppSettingsPath = ProjectRootDirectory + "/" + AppSettingsFileName;
    private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .AddJsonFile(AppSettingsPath)
        .AddEnvironmentVariables()
        .Build();

    private static string GetProjectRootDirectory()
    {
        string curPath = Directory.GetCurrentDirectory();
        var directoryInfo = Directory.GetParent(curPath);

        for (int i = 0; i < 2; i++)
        {
            if (directoryInfo != null)
            {
                directoryInfo = directoryInfo.Parent;
            }
            else
            {
                break;
            }
        }

        return directoryInfo?.FullName ?? string.Empty;
    }

    public static string GetDatabaseNameFromConfig()
    {
        var section = Config.GetSection("Database:Name");

        return section.Value ?? string.Empty;
    }
}