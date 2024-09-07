using System.Globalization;
using Spectre.Console;

namespace CodingTracker.Utils;

public static class Prompts
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    
    public static DateTime DatePrompt(string type, DateTime startTime = default)
    {
        var timePrompt = new TextPrompt<string>(
            $"Enter the [aqua bold]{type} (including date)[/] of your coding session in " +
            $"[aqua bold]{DateTimeFormat}[/] format: ")
            .Validate(value => CodingTracker.Utils.Helper.ValidateDateTime(value, DateTimeFormat, type, startTime))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));
        string dateTimeString = AnsiConsole.Prompt(timePrompt);
        
        DateTime.TryParseExact(
            dateTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime);

        return dateTime;
    }

    public static int RecordSelectionPrompt(HashSet<int> recordIds, string action)
    {
        var idPrompt = new TextPrompt<int>(
            "Enter the [aqua bold]id[/] of the coding session " +
            $"which you want to [aqua bold]{action}[/]: ")
            .Validate(value => CodingTracker.Utils.Helper.ValidateId(value, recordIds))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));

        int idOfSelectedRecord = AnsiConsole.Prompt(idPrompt);

        return idOfSelectedRecord;
    }

    public static char FilterSelectionPrompt()
    {
        string filterOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select your [green]viewing option[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "1. View all records", "2. View records in the current year", 
                    "3. View records in the current month", "4. View records in the current week"
                ]));

        return filterOption.ToCharArray()[0];
    }

    public static char SortingSelectionPrompt()
    {
        string sortingOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select your [green]sorting option[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "1. Ascending", "2. Descending"
                ]));

        return sortingOption.ToCharArray()[0];
    }
}