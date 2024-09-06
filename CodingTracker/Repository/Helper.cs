using CodingTracker.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace CodingTracker.Repository;

public static class Helper
{
    public static List<CodingSession> GetCodingSessions(SqliteConnection connection)
    {
        var sessions = connection.Query<CodingSession>(QueriesAndCommands.ViewAllRecords).ToList();

        return sessions;
    }

    public static void DisplayAllCodingSessions(List<CodingSession> sessions)
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
                new Markup($"{Utils.Utils.ConvertSecondsToHoursMinutesSeconds(Convert.ToInt32(session.Duration))}"));
        }

        Console.WriteLine();
        AnsiConsole.Write(table);
    }
    
    public static void DisplayBreakdownByMonthForCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Now.Year;
        var occurrences =
            connection.Query<MonthlyOccurrence>(
                    QueriesAndCommands.GetTotalNumOfCodingSessionsForEachMonthOfTheCurrentYear, 
                    new MonthlyOccurrence { Year = currentYear.ToString() })
                .ToList();
        var monthsDictionary = new Dictionary<int, (string, Color)>
        {
            { 1, ("January", Color.Yellow)},
            { 2, ("February", Color.Blue) },
            { 3, ("March", Color.Cyan1) },
            { 4, ("April", Color.Red) },
            { 5, ("May", Color.Fuchsia) },
            { 6, ("June", Color.Turquoise2) },
            { 7, ("July", Color.Orange1) },
            { 8, ("August", Color.Purple) },
            { 9, ("September", Color.Magenta2) },
            { 10, ("October", Color.HotPink) },
            { 11, ("November", Color.Chartreuse1) },
            { 12, ("December", Color.Teal) }
        };

        // Create a list of fruits
        var items = new List<(string Label, long Value, Color Color)>();

        foreach (var occurrence in occurrences)
        {
            var tuple = monthsDictionary[occurrence.Month];
            (string monthName, var color) = tuple;
            items.Add((monthName, occurrence.Occurrences, color));
        }
        
        // Render bar chart
        AnsiConsole.Write(new BarChart()
            .Width(100)
            .Label($"[green bold underline]Monthly breakdown for {currentYear}[/]")
            .CenterLabel()
            .AddItems(items, (item) => new BarChartItem(
                item.Label, item.Value, item.Color)));
    }
    
    public static void GetOverallStatistics(SqliteConnection connection)
    {
        int totalNumOfCodingSessions = GetTotalNumOfCodingSessions(connection);
        string totalTimeSpentCodingInHoursMinutesSeconds = GetTotalTimeSpentCoding(connection);
        int totalNumOfCodingSessionsInCurrentYear = GetTotalNumOfCodingSessionsInTheCurrentYear(connection);
        string totalTimeSpentCodingInCurrentYear = GetTotalTimeSpentCodingInTheCurrentYear(connection);
        int totalNumOfCodingSessionsInCurrentMonth = GetTotalNumOfCodingSessionsInTheCurrentMonth(connection);
        string totalTimeSpentCodingInCurrentMonth = GetTotalTimeSpentCodingInTheCurrentMonth(connection);
        int totalNumOfCodingSessionsInCurrentWeek = GetTotalNumOfCodingSessionsInTheCurrentWeek(connection);
        string totalTimeSpentCodingInCurrentWeek = GetTotalTimeSpentCodingInTheCurrentWeek(connection);

        var rows = new List<Text>
        {
            new($"Total overall number of coding sessions: \t\t{totalNumOfCodingSessions}"),
            new($"\nTotal overall duration spent coding: \t\t\t{totalTimeSpentCodingInHoursMinutesSeconds}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current year: \t{totalNumOfCodingSessionsInCurrentYear}"),
            new($"\nTotal duration spent coding in the current year: \t{totalTimeSpentCodingInCurrentYear}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current month: \t{totalNumOfCodingSessionsInCurrentMonth}"),
            new($"\nTotal duration spent coding in the current month: \t{totalTimeSpentCodingInCurrentMonth}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current week: \t{totalNumOfCodingSessionsInCurrentWeek}"),
            new($"\nTotal duration spent coding in the current week: \t{totalTimeSpentCodingInCurrentWeek}"),
            new("\n")
        };
        
        AnsiConsole.MarkupLine("\n[underline aqua]Overall Statistics[/]\n");
        foreach (var row in rows)
        {
            AnsiConsole.Write(row);
        }
    }
    
     private static string GetTotalTimeSpentCoding(SqliteConnection connection)
    {
        int totalTimeSpentCodingInSeconds = connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalTimeSpentCoding);
        string totalTimeSpentCodingInHoursMinutesSeconds = 
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInSeconds);

        return totalTimeSpentCodingInHoursMinutesSeconds;
    }

    private static int GetTotalNumOfCodingSessions(SqliteConnection connection)
    {
        int totalNumOfCodingSessions = connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalNumOfCodingSessions);

        return totalNumOfCodingSessions;
    }

    private static string GetTotalTimeSpentCodingInTheCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, start, end);
    }

    private static string GetTotalTimeSpentCodingInTheCurrentMonth(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, firstDayOfMonth, lastDayOfMonth);
    }

    private static string GetTotalTimeSpentCodingInTheCurrentWeek(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, startOfWeek, endOfWeek);
    }
    
    private static string GetTotalTimeSpentCodingWithinATimePeriod(SqliteConnection connection, DateTime start,
        DateTime end)
    {
        var condition = new CodingSession() { StartTime = start, EndTime = end };

        int totalTimeSpentCodingWithinATimePeriodInSeconds =
            connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalTimeSpentCodingWithinATimePeriod, condition);
        string totalTimeSpentCodingWithinATimePeriodInHoursMinutesSeconds =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingWithinATimePeriodInSeconds);

        return totalTimeSpentCodingWithinATimePeriodInHoursMinutesSeconds;
    }

    private static int GetTotalNumOfCodingSessionsInTheCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);
        
        return GetTotalNumOfCodingSessionsWithinATimePeriod(connection, start, end);
    }

    private static int GetTotalNumOfCodingSessionsInTheCurrentMonth(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return GetTotalNumOfCodingSessionsWithinATimePeriod(connection, firstDayOfMonth, lastDayOfMonth);
    }
    
    private static int GetTotalNumOfCodingSessionsInTheCurrentWeek(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetTotalNumOfCodingSessionsWithinATimePeriod(connection, startOfWeek, endOfWeek);
    }

    private static int GetTotalNumOfCodingSessionsWithinATimePeriod(SqliteConnection connection, DateTime start, DateTime end)
    {
        var condition = new CodingSession() { StartTime = start, EndTime = end };
        
        int totalNumOfCodingSessionsWithinATimePeriod = 
            connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalNumOfCodingSessionsWithinATimePeriod, condition);

        return totalNumOfCodingSessionsWithinATimePeriod;
    }
}