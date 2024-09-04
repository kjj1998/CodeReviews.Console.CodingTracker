using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace CodingTracker;

public class Respository
{
    public static void ViewAllRecords(SqliteConnection connection)
    {
        string viewAllRecordsQuery =
            Utils.Config.GetSection("Database:Queries:ViewAllRecords").Value ?? string.Empty;
        var sessions = connection.Query<CodingSession>(viewAllRecordsQuery);
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
}