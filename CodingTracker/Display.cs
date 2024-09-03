namespace CodingTracker;

using Spectre.Console;

public static class Display
{
    public static void WelcomeDisplay()
    {
        AnsiConsole.MarkupLine("\n[steelblue1]------------------------------------------------[/]");
        AnsiConsole.MarkupLine(":open_book: [steelblue1]Welcome to the Coding Tracker Application![/] :open_book:");
        AnsiConsole.MarkupLine("[steelblue1]------------------------------------------------[/]");
        Console.WriteLine("This application allows you to record down and track your coding sessions.");
        Console.WriteLine("You can also update or delete existing coding sessions.");
        Console.WriteLine("A summary of all of your coding sessions can also be viewed.");
    }

    public static void MenuDisplay()
    {
        var selectionPrompt = new SelectionPrompt<string>
        {
            Title = "\n[bold]What would you like to [green]do[/][/]?",
            PageSize = 10,
            MoreChoicesText = "[grey](Move up and down to reveal more options)[/]",
            HighlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold)
        };
        selectionPrompt.AddChoices([
            "View all coding sessions",
            "Create a new coding session",
            "Update an existing coding session",
            "Delete an existing coding session",
            "View a summary of your coding sessions",
            "Exit the application"
        ]);

        string option = AnsiConsole.Prompt(selectionPrompt);
        option = LowerCaseFirstWord(option);
        
        AnsiConsole.WriteLine($"\nYou have chosen to {option}.");
    }

    private static string LowerCaseFirstWord(string input)
    {
        string[] words = input.Split(" ");
        words[0] = words[0].ToLower();

        return string.Join(" ", words);
    }
}