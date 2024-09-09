using CodingTracker.Model;
using CodingTracker.Repository;
using CodingTracker.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using Helper = CodingTracker.Utils.Helper;

namespace CodingTracker.Controller;

public static class SessionController
{
    public static void ViewAllSessions(SqliteConnection connection)
    {
        char filterOption = Prompts.FilterSelectionPrompt();
        char sortingOption = Prompts.SortingSelectionPrompt();
        var sessions = CodingSessionRepo.GetCodingSessions(
            connection, filterOption, sortingOption);

        if (sessions.Count == 0)
            Console.WriteLine("\nThere are no coding sessions! Please enter some!");
        else
            CodingSessionRepo.DisplayAllCodingSessions(sessions);

        Helper.UserAcknowledgement();
    }
    
    public static void InsertSession(SqliteConnection connection)
    {
        var startTime = Prompts.DatePrompt("start time");
        var endTime = Prompts.DatePrompt("end time", startTime);
        int duration = Helper.CalculateDuration(startTime, endTime);
        
        var session = new Session() { StartTime = startTime, EndTime = endTime, Duration = duration };
        int rowsAffected = connection.Execute(Query.Session.InsertRecord, session);

        if (rowsAffected == 1)
            AnsiConsole.MarkupLine("\n[steelblue1 bold]Coding Session successfully entered![/]");
        
        Helper.UserAcknowledgement();
    }

    public static void UpdateSession(SqliteConnection connection)
    {
        var sessions = CodingSessionRepo.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions available for update!");
        }
        else
        {
            var sessionIds = sessions.Select(session => session.Id).ToHashSet();
            CodingSessionRepo.DisplayAllCodingSessions(sessions);
            long sessionToEdit = Prompts.SessionSelectionPrompt(sessionIds, "edit");

            AnsiConsole.MarkupLine(
                $"You have selected to edit the coding session with id [aqua bold]{sessionToEdit}[/]");

            var startTime = Prompts.DatePrompt("start time");
            var endTime = Prompts.DatePrompt("end time", startTime);
            int duration = Helper.CalculateDuration(startTime, endTime);

            var updatedCodingSession = new Session()
            {
                Id = sessionToEdit,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration
            };

            int rowsAffected = connection.Execute(Query.Session.UpdateSession, updatedCodingSession);

            if (rowsAffected == 1)
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Coding Session with " +
                                       $"id {sessionToEdit} successfully updated![/]");

            Helper.UserAcknowledgement();
        }
    }

    public static void DeleteSession(SqliteConnection connection)
    {
        var sessions = CodingSessionRepo.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions to delete!");
        }
        else
        {
            var sessionIds = sessions.Select(session => session.Id).ToHashSet();
            CodingSessionRepo.DisplayAllCodingSessions(sessions);

            foreach (var session in sessions)
                sessionIds.Add(Convert.ToInt32(session.Id));

            long sessionToDelete = Prompts.SessionSelectionPrompt(sessionIds, "delete");
            AnsiConsole.MarkupLine(
                $"You have selected to delete the coding session with id [aqua bold]{sessionToDelete}[/]");

            var codingSessionToBeDeleted = new Session() { Id = sessionToDelete };
            int rowsAffected = connection.Execute(Query.Session.DeleteSession, codingSessionToBeDeleted);

            if (rowsAffected == 1)
                AnsiConsole.MarkupLine(
                    $"\n[steelblue1 bold]Coding Session with id {sessionToDelete} successfully deleted![/]");

            Helper.UserAcknowledgement();
        }
    }
    
    public static void ViewCodingSessionsSummary(SqliteConnection connection)
    {
        var sessions = CodingSessionRepo.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions to view summary!");
        }
        else
        {
            CodingSessionRepo.DisplayStatistics(connection);
            Console.WriteLine();
            CodingSessionRepo.DisplayBreakdownByMonthForCurrentYear(connection);
        }
        
        Helper.UserAcknowledgement();
    }
    
     public static void LiveCodingSession(SqliteConnection connection)
    {
        var startTime = DateTime.Now;
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var endTime = DateTime.Now;
        
        Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    cancellationTokenSource.Cancel();
                    endTime = DateTime.Now;
                }
                Thread.Sleep(100);
            }
        }, token);
        
        int initialCursorLeft = Console.CursorLeft;
        int initialCursorTop = Console.CursorTop;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var elapsedTime = DateTime.Now - startTime;
                Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
                AnsiConsole.MarkupLine($@"Elapsed Time: [aqua bold]{elapsedTime:hh\:mm\:ss}[/]");
                Console.SetCursorPosition(initialCursorLeft + 22, initialCursorTop);
                Thread.Sleep(1000);
            }
        }
        finally
        {
            Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
            Console.WriteLine($"Timer stopped. You have coded from {startTime.ToLongTimeString()} to {endTime.ToLongTimeString()}"); 
            
            int duration = Helper.CalculateDuration(startTime, endTime);
            var codingSession = new Session() { StartTime = startTime, EndTime = endTime, Duration = duration };
            int rowsAffected = connection.Execute(Query.Session.InsertRecord, codingSession);

            if (rowsAffected == 1)
                AnsiConsole.MarkupLine("\n[steelblue1 bold]Live Coding Session successfully saved![/]");
        }
        
        Helper.UserAcknowledgement();
    }
}