﻿using CodingTracker;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

try
{
    var connection = StartUp.SystemStartUpCheck();
    bool exitApp = false;
    
    Display.WelcomeDisplay();

    while (exitApp == false)
    {
        char option = Display.MenuDisplay().ToCharArray()[0];

        switch (option)
        {
            case '1':
                Respository.ViewAllRecords(connection);
                break;
            case '2':
                break;
            case '3':
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



