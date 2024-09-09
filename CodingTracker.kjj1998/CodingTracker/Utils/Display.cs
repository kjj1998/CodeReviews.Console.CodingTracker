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
        List<string> options =
        [
            "1. View all coding sessions",
            "2. Create a new coding session",
            "3. Update an existing coding session",
            "4. Delete an existing coding session",
            "5. View a summary of your coding sessions",
            "6. Start a live coding session",
            "7. View/Edit coding goals",
            "8. Exit the application"
        ];
        var highlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold);
        const string instruction = "\n[bold]What would you like to [green]do[/][/]?";
        string option = Utils.Prompts.OptionSelectionPrompt(options, instruction, highlightStyle);
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