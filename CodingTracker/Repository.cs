using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace CodingTracker;

public static class Repository
{
    private static List<CodingSession> GetCodingSessions(SqliteConnection connection)
    {
        string viewAllRecordsQuery =
            Utils.Config.GetSection("Database:Queries:ViewAllRecords").Value ?? string.Empty;
        var sessions = connection.Query<CodingSession>(viewAllRecordsQuery).ToList();

        return sessions;
    }

    private static void DisplayAllCodingSessions(List<CodingSession> sessions)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn(new TableColumn("Start").Centered());
        table.AddColumn(new TableColumn("End").Centered());
        table.AddColumn(new TableColumn("Duration").RightAligned());
                
        table.Columns[1].Width(30);
        table.Columns[2].Width(30);
        table.Columns[3].Width(20);
        
        foreach (var session in sessions)
        {
            table.AddRow(
                new Markup($"{session.Id}"),
                new Markup($"{session.StartTime}"),
                new Markup($"{session.EndTime}"),
                new Markup($"{Utils.ConvertSecondsToHoursMinutesSeconds(Convert.ToInt32(session.Duration))}"));
        }
                
        Console.WriteLine();
        AnsiConsole.Write(table);
    }
    
    public static void ViewAllRecords(SqliteConnection connection)
    {
        var sessions = GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions! Please enter some!");
        }
        else
        {
            DisplayAllCodingSessions(sessions);
        }
    }

    public static async void InsertRecord(SqliteConnection connection)
    {
        var startTime = Prompts.DatePrompt("start time");
        var endTime = Prompts.DatePrompt("end time", startTime);
        int duration = Utils.CalculateDuration(startTime, endTime);
    
        string insertRecordCommand = 
            Utils.Config.GetSection("Database:Commands:InsertRecord").Value ?? string.Empty;
        
        var codingSession = new CodingSession() { StartTime = startTime, EndTime = endTime, Duration = duration};
        int rowsAffected = await connection.ExecuteAsync(insertRecordCommand, codingSession);

        if (rowsAffected == 1)
        {
            AnsiConsole.MarkupLine("\n[steelblue1 bold]Coding Session successfully entered![/]");
        }
    }

    public static async void UpdateRecord(SqliteConnection connection)
    {
        var sessions = GetCodingSessions(connection);

        if (sessions.Count == 0)
        {
            Console.WriteLine("\nThere are no coding sessions available for update!");
        }
        else
        {
            HashSet<int> setOfRecordIds = [];
            DisplayAllCodingSessions(sessions);

            foreach (var session in sessions)
            {
                setOfRecordIds.Add(Convert.ToInt32(session.Id));
            }
            
            int recordToEdit = Prompts.RecordSelectionPrompt(setOfRecordIds);
            AnsiConsole.MarkupLine($"You have selected to edit the coding session with id [aqua bold]{recordToEdit}[/]");
            
            var startTime = Prompts.DatePrompt("start time");
            var endTime = Prompts.DatePrompt("end time", startTime);
            int duration = Utils.CalculateDuration(startTime, endTime);
            string updateRecordCommand = 
                Utils.Config.GetSection("Database:Commands:UpdateRecord").Value ?? string.Empty;

            var updatedCodingSession = new CodingSession() {
                Id = recordToEdit,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration
            };

            int rowsAffected = await connection.ExecuteAsync(updateRecordCommand, updatedCodingSession);
            
            if (rowsAffected == 1)
            {
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Coding Session with id {recordToEdit} successfully updated![/]");
            }
        }
    }
}