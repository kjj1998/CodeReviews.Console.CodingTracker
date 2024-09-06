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
        int totalTimeSpentCodingInSeconds = GetTotalTimeSpentCoding(connection);
        string totalTimeSpentCodingInHoursMinutesSeconds = 
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInSeconds);
        int averageDuration = (int) Math.Round(totalTimeSpentCodingInSeconds / (double)totalNumOfCodingSessions);
        string averageDurationInHoursMinutesSeconds = Utils.Utils.ConvertSecondsToHoursMinutesSeconds(averageDuration);
        
        int totalNumOfCodingSessionsInCurrentYear = GetTotalNumOfCodingSessionsInTheCurrentYear(connection);
        int totalTimeSpentCodingInCurrentYear = GetTotalTimeSpentCodingInTheCurrentYear(connection);
        string totalTimeSpentCodingInCurrentYearInHoursMinutesSeconds =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentYear);
        int averageDurationSpentCodingInCurrentYear = 
            (int) Math.Round(totalTimeSpentCodingInCurrentYear / (double)totalNumOfCodingSessionsInCurrentYear);
        string averageDurationSpentCodingInCurrentYearInHms =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentYear);
        
        int totalNumOfCodingSessionsInCurrentMonth = GetTotalNumOfCodingSessionsInTheCurrentMonth(connection);
        int totalTimeSpentCodingInCurrentMonth = GetTotalTimeSpentCodingInTheCurrentMonth(connection);
        string totalTimeSpentCodingInCurrentMonthInHms =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentMonth);
        int averageDurationSpentCodingInCurrentMonth
            = (int)Math.Round(totalTimeSpentCodingInCurrentMonth / (double)totalNumOfCodingSessionsInCurrentMonth);
        string averageDurationSpentCodingInCurrentMonthInHms =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentMonth);
        
        int totalNumOfCodingSessionsInCurrentWeek = GetTotalNumOfCodingSessionsInTheCurrentWeek(connection);
        int totalTimeSpentCodingInCurrentWeek = GetTotalTimeSpentCodingInTheCurrentWeek(connection);
        string totalTimeSpentCodingInCurrentWeekInHms =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentWeek);
        int averageDurationSpentCodingInCurrentWeek =
            (int)Math.Round(totalTimeSpentCodingInCurrentWeek / (double)totalNumOfCodingSessionsInCurrentWeek);
        string averageDurationSpentCodingInCurrentWeekInHms =
            Utils.Utils.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentWeek);

        var rows = new List<Text>
        {
            new($"Total overall number of coding sessions: \t\t{totalNumOfCodingSessions}"),
            new($"\nTotal overall duration spent coding: \t\t\t{totalTimeSpentCodingInHoursMinutesSeconds}"),
            new($"\nAverage time spent coding per session: \t\t\t{averageDurationInHoursMinutesSeconds}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current year: \t{totalNumOfCodingSessionsInCurrentYear}"),
            new($"\nTotal duration spent coding in the current year: \t{totalTimeSpentCodingInCurrentYearInHoursMinutesSeconds}"),
            new($"\nAverage time spent coding in the current year: \t\t{averageDurationSpentCodingInCurrentYearInHms}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current month: \t{totalNumOfCodingSessionsInCurrentMonth}"),
            new($"\nTotal duration spent coding in the current month: \t{totalTimeSpentCodingInCurrentMonthInHms}"),
            new($"\nAverage time spent coding in the current month: \t{averageDurationSpentCodingInCurrentMonthInHms}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current week: \t{totalNumOfCodingSessionsInCurrentWeek}"),
            new($"\nTotal duration spent coding in the current week: \t{totalTimeSpentCodingInCurrentWeekInHms}"),
            new($"\nAverage time spent coding in the current month: \t{averageDurationSpentCodingInCurrentWeekInHms}"),
            new("\n")
        };
        
        AnsiConsole.MarkupLine("\n[underline aqua]Overall Statistics[/]\n");
        foreach (var row in rows)
        {
            AnsiConsole.Write(row);
        }
    }
    
     private static int GetTotalTimeSpentCoding(SqliteConnection connection)
    {
        int totalTimeSpentCodingInSeconds = connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalTimeSpentCoding);

        return totalTimeSpentCodingInSeconds;
    }

    private static int GetTotalNumOfCodingSessions(SqliteConnection connection)
    {
        int totalNumOfCodingSessions = connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalNumOfCodingSessions);

        return totalNumOfCodingSessions;
    }

    private static int GetTotalTimeSpentCodingInTheCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, start, end);
    }

    private static int GetTotalTimeSpentCodingInTheCurrentMonth(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, firstDayOfMonth, lastDayOfMonth);
    }

    private static int GetTotalTimeSpentCodingInTheCurrentWeek(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, startOfWeek, endOfWeek);
    }
    
    private static int GetTotalTimeSpentCodingWithinATimePeriod(SqliteConnection connection, DateTime start,
        DateTime end)
    {
        var condition = new CodingSession() { StartTime = start, EndTime = end };

        int totalTimeSpentCodingWithinATimePeriodInSeconds =
            connection.ExecuteScalar<int>(QueriesAndCommands.GetTotalTimeSpentCodingWithinATimePeriod, condition);

        return totalTimeSpentCodingWithinATimePeriodInSeconds;
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