using CodingTracker;
using CodingTracker.Controller;
using CodingTracker.Repository;
using CodingTracker.Utils;
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
                SessionController.ViewAllSessions(connection);
                break;
            case '2':
                SessionController.InsertSession(connection);
                break;
            case '3':
                SessionController.UpdateSession(connection);
                break;
            case '4':
                SessionController.DeleteSession(connection);
                break;
            case '5':
                SessionController.ViewCodingSessionsSummary(connection);
                break;
            case '6':
                SessionController.LiveCodingSession(connection);
                break;
            case '7':
                GoalController.SetCodingGoals(connection);
                break;
            case '8':
                exitApp = true;
                break;
        }
    }
    
}
catch (SqliteException ex)
{
    AnsiConsole.WriteException(ex);
}



