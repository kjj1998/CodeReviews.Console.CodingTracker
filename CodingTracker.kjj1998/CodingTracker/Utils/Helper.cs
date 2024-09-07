using System.Globalization;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

// ReSharper disable InvertIf

namespace CodingTracker.Utils;

public static class Helper
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

    public static string ConvertSecondsToHoursMinutesSeconds(int duration)
    {
        var timespan = TimeSpan.FromSeconds(duration);

        int hours = timespan.Hours;
        int minutes = timespan.Minutes;
        int seconds = timespan.Seconds;

        return $"{hours} h {minutes} min {seconds} sec";
    }

    public static ValidationResult ValidateDateTime(string dateTimeString, string dateTimeFormat, string type, DateTime startTime)
    {
        bool checkDateTime = DateTime.TryParseExact(
            dateTimeString, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime);

        if (checkDateTime)
        {
            if (type == "end time")
            {
                return dateTime < startTime ? 
                    ValidationResult.Error("End time must be greater than start time.") : 
                    ValidationResult.Success();
            }

            return ValidationResult.Success();
        }

        return ValidationResult.Error($"Please enter a valid date-time in the format {dateTimeFormat}.");
    }

    public static ValidationResult ValidateId(int id, HashSet<int> recordIds)
    {
        return recordIds.Contains(id) ? 
            ValidationResult.Success() : 
            ValidationResult.Error($"Please enter a valid id that is shown in the table above.");
    }
    
    public static int CalculateDuration(DateTime startTime, DateTime endTime)
    {
        var difference = endTime - startTime;
        int durationInSeconds = difference.Hours * 3600 + difference.Minutes * 60 + difference.Seconds;

        return durationInSeconds;
    }

    public static void UserAcknowledgement()
    {
        bool readKeyToContinue = false;
        
        while (!readKeyToContinue)
        {
            Console.Write("Press enter to continue ... ");
            string? readKey = Console.ReadLine();

            if (readKey == null) continue;
            readKeyToContinue = true;
            Console.Clear();
        }
    }
}