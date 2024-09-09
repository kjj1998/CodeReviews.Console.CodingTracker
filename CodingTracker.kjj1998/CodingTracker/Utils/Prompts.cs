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
            .Validate(value => Helper.ValidateDateTime(value, DateTimeFormat, type, startTime))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));
        string dateTimeString = AnsiConsole.Prompt(timePrompt);
        
        DateTime.TryParseExact(
            dateTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime);

        return dateTime;
    }

    public static double DoubleValuePrompt(string instruction)
    {
        var doubleValuePrompt = new TextPrompt<string>(instruction)
            .Validate(value => double.TryParse(value, out double _))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));

        string val = AnsiConsole.Prompt(doubleValuePrompt);

        double.TryParse(val, out double doubleVal);
        
        return doubleVal;
    }

    public static int RecordSelectionPrompt(HashSet<int> recordIds, string action)
    {
        var idPrompt = new TextPrompt<int>(
            "Enter the [aqua bold]id[/] of the coding session " +
            $"which you want to [aqua bold]{action}[/]: ")
            .Validate(value => Helper.ValidateId(value, recordIds))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));

        int idOfSelectedRecord = AnsiConsole.Prompt(idPrompt);

        return idOfSelectedRecord;
    }
    
    public static long SessionSelectionPrompt(HashSet<long> sessionIds, string action)
    {
        var idPrompt = new TextPrompt<long>(
                "Enter the [aqua bold]id[/] of the coding session " +
                $"which you want to [aqua bold]{action}[/]: ")
            .Validate(value => Helper.ValidateId(value, sessionIds))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));

        long idOfSelectedRecord = AnsiConsole.Prompt(idPrompt);

        return idOfSelectedRecord;
    }
    
    public static long GoalSelectionPrompt(HashSet<long> goalIds, string action)
    {
        var goalPrompt = new TextPrompt<long>(
                "Enter the [aqua bold]id[/] of the goal " +
                $"which you want to [aqua bold]{action}[/]: ")
            .Validate(value => Helper.ValidateId(value, goalIds))
            .PromptStyle(new Style(foreground:Color.Aqua, decoration:Decoration.Bold));

        long idOfSelectedGoal = AnsiConsole.Prompt(goalPrompt);

        return idOfSelectedGoal;
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
    
    public static string OptionSelectionPrompt(List<string> options, string instruction, Style? highlightStyle = null)
    {
        string selectedOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(instruction)
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(options)
                .HighlightStyle(highlightStyle ?? Style.Plain)
            );

        return selectedOption;
    }
}