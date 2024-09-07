using CodingTracker.Models;
using CodingTracker.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace CodingTracker.Repository;

public static class Repository
{ 
    public static void ViewAllRecords(SqliteConnection connection)
    {
        var sessions = Helper.GetCodingSessions(connection);

        if (sessions.Count == 0)
            Console.WriteLine("\nThere are no coding sessions! Please enter some!");
        else
            Helper.DisplayAllCodingSessions(sessions);

        Utils.Helper.UserAcknowledgement();
    }

    public static async void InsertRecord(SqliteConnection connection)
    {
        var startTime = Prompts.DatePrompt("start time");
        var endTime = Prompts.DatePrompt("end time", startTime);
        int duration = Utils.Helper.CalculateDuration(startTime, endTime);
        
        var codingSession = new CodingSession() { StartTime = startTime, EndTime = endTime, Duration = duration };
        int rowsAffected = await connection.ExecuteAsync(QueriesAndCommands.InsertRecord, codingSession);

        if (rowsAffected == 1)
            AnsiConsole.MarkupLine("\n[steelblue1 bold]Coding Session successfully entered![/]");
        
        Utils.Helper.UserAcknowledgement();
    }

    public static async void UpdateRecord(SqliteConnection connection)
    {
        var sessions = Helper.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions available for update!");
        }
        else
        {
            HashSet<int> setOfRecordIds = [];
            Helper.DisplayAllCodingSessions(sessions);

            foreach (var session in sessions)
            {
                setOfRecordIds.Add(Convert.ToInt32(session.Id));
            }

            int recordToEdit = Prompts.RecordSelectionPrompt(setOfRecordIds, "edit");
            AnsiConsole.MarkupLine(
                $"You have selected to edit the coding session with id [aqua bold]{recordToEdit}[/]");

            var startTime = Prompts.DatePrompt("start time");
            var endTime = Prompts.DatePrompt("end time", startTime);
            int duration = Utils.Helper.CalculateDuration(startTime, endTime);
            
            var updatedCodingSession = new CodingSession()
            {
                Id = recordToEdit,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration
            };

            int rowsAffected = await connection.ExecuteAsync(QueriesAndCommands.UpdateRecord, updatedCodingSession);

            if (rowsAffected == 1)
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Coding Session with id {recordToEdit} successfully updated![/]");
            
            Utils.Helper.UserAcknowledgement();
        }
    }

    public static async void DeleteRecord(SqliteConnection connection)
    {
        var sessions = Helper.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions to delete!");
        }
        else
        {
            HashSet<int> setOfRecordIds = [];
            Helper.DisplayAllCodingSessions(sessions);

            foreach (var session in sessions)
                setOfRecordIds.Add(Convert.ToInt32(session.Id));

            int recordToDelete = Prompts.RecordSelectionPrompt(setOfRecordIds, "delete");
            AnsiConsole.MarkupLine(
                $"You have selected to delete the coding session with id [aqua bold]{recordToDelete}[/]");

            var codingSessionToBeDeleted = new CodingSession() { Id = recordToDelete };
            int rowsAffected = await connection.ExecuteAsync(QueriesAndCommands.DeleteRecord, codingSessionToBeDeleted);

            if (rowsAffected == 1)
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Coding Session with id {recordToDelete} successfully deleted![/]");
            
            Utils.Helper.UserAcknowledgement();
        }
    }

    public static void ViewCodingSessionsSummary(SqliteConnection connection)
    {
        var sessions = Helper.GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions to view summary!");
        }
        else
        {
            Helper.GetOverallStatistics(connection);
            Console.WriteLine();
            Helper.DisplayBreakdownByMonthForCurrentYear(connection);
        }
        
        Utils.Helper.UserAcknowledgement();
    }
}