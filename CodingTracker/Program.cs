using CodingTracker;
using Microsoft.Data.Sqlite;
using Spectre.Console;

try
{
    var connection = StartUp.SystemStartUpCheck();
    bool exitApp = false;
    
    Display.WelcomeDisplay();

    while (exitApp == false)
    {
        char option = Display.MenuDisplay();

        switch (option)
        {
            case '1':
                Repository.ViewAllRecords(connection);
                break;
            case '2':
                Repository.InsertRecord(connection);
                break;
            case '3':
                Repository.UpdateRecord(connection);
                break;
            case '4':
                break;
            case '5':
                break;
            case '6':
                exitApp = true;
                break;
        }
    }
    
}
catch (SqliteException ex)
{
    AnsiConsole.WriteException(ex);
}



