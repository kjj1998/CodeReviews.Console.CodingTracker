using CodingTracker;
using Microsoft.Data.Sqlite;
using Spectre.Console;

try
{
    var connection = StartUp.SystemStartUpCheck();

    Display.WelcomeDisplay();
    Display.MenuDisplay();
}
catch (SqliteException ex)
{
    AnsiConsole.WriteException(ex);
}



