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

    public static char MenuDisplay()
    {
        var selectionPrompt = new SelectionPrompt<string>
        {
            Title = "\n[bold]What would you like to [green]do[/][/]?",
            PageSize = 10,
            MoreChoicesText = "[grey](Move up and down to reveal more options)[/]",
            HighlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold)
        };
        selectionPrompt.AddChoices([
            "1. View all coding sessions",
            "2. Create a new coding session",
            "3. Update an existing coding session",
            "4. Delete an existing coding session",
            "5. View a summary of your coding sessions",
            "6. Exit the application"
        ]);

        string option = AnsiConsole.Prompt(selectionPrompt);
        string message = LowerCaseFirstWord(option.Substring(3, option.Length - 3));
        
        AnsiConsole.WriteLine($"\nYou have chosen to {message}.");

        return option.ToCharArray()[0];
    }

    private static string LowerCaseFirstWord(string input)
    {
        string[] words = input.Split(" ");
        words[0] = words[0].ToLower();

        return string.Join(" ", words);
    }
}