using CodingTracker.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using static CodingTracker.Repository.DatabaseAccess;

namespace CodingTracker.Repository;

public static class CodingSessionRepo
{
    public static List<Session> GetCodingSessions(
        SqliteConnection connection, 
        char filterOption = '1', 
        char sortingOption = '1')
    {
        var sessions = new List<Session>();

        string query = filterOption switch
        {
            '1' => Query.Session.ViewAllRecords,
            _ => Query.Session.ViewAllRecordsWithinATimePeriod
        };

        switch (sortingOption)
        {
            case '1':
                query += " ORDER BY startTime ASC";
                break;
            case '2':
                query += " ORDER BY startTime DESC";
                break;
        }

        sessions = filterOption switch
        {
            '1' => GetAllSessions(connection, query),
            '2' => GetAllSessionsInCurrentYear(connection, query),
            '3' => GetAllSessionsInCurrentMonth(connection, query),
            '4' => GetAllSessionsInCurrentWeek(connection, query),
            _ => sessions
        };

        return sessions;
    }
    
    public static void DisplayAllCodingSessions(List<Session> sessions)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn(new TableColumn("Start").Centered().Width(30));
        table.AddColumn(new TableColumn("End").Centered().Width(30));
        table.AddColumn(new TableColumn("Duration").RightAligned().Width(20));

        foreach (var session in sessions)
        {
            table.AddRow(
                new Markup($"{session.Id}"),
                new Markup($"{session.StartTime}"),
                new Markup($"{session.EndTime}"),
                new Markup($"{Utils.Helper.ConvertSecondsToHoursMinutesSeconds(
                    Convert.ToInt32(session.Duration))}"));
        }

        Console.WriteLine();
        AnsiConsole.Write(table);
    }

    public static void DisplayStatistics(SqliteConnection connection)
    {
        int totalNumOfSessions = GetTotalNumOfSessions(connection);
        int totalNumOfSessionsInCurrentYear = GetTotalNumOfSessionsInTheCurrentYear(connection);
        int totalNumOfSessionsInCurrentMonth = GetTotalNumOfSessionsInTheCurrentMonth(connection);
        int totalNumOfSessionsInCurrentWeek = GetTotalNumOfSessionsInTheCurrentWeek(connection);
        
        int totalTimeSpentCodingInSeconds = GetTotalTimeSpentCoding(connection);
        int totalTimeSpentCodingInCurrentYear = GetTotalTimeSpentCodingInTheCurrentYear(connection);
        int totalTimeSpentCodingInCurrentMonth = GetTotalTimeSpentCodingInTheCurrentMonth(connection);
        int totalTimeSpentCodingInCurrentWeek = GetTotalTimeSpentCodingInTheCurrentWeek(connection);
        
        string totalTimeSpentCodingInHoursMinutesSeconds =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInSeconds);
        string totalTimeSpentCodingInCurrentYearInHoursMinutesSeconds =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentYear);
        string totalTimeSpentCodingInCurrentMonthInHms =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentMonth);
        string totalTimeSpentCodingInCurrentWeekInHms =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(totalTimeSpentCodingInCurrentWeek);
        
        string averageDurationInHoursMinutesSeconds =
            Helper.AverageDurationInHoursMinutesSeconds(totalTimeSpentCodingInSeconds, totalNumOfSessions);
        string averageDurationSpentCodingInCurrentYearInHms =
            Helper.AverageDurationSpentCodingInCurrentYearInHms(totalTimeSpentCodingInCurrentYear, totalNumOfSessionsInCurrentYear);
        string averageDurationSpentCodingInCurrentMonthInHms =
            Helper.AverageDurationSpentCodingInCurrentMonthInHms(totalTimeSpentCodingInCurrentMonth, totalNumOfSessionsInCurrentMonth);
        string averageDurationSpentCodingInCurrentWeekInHms =
            Helper.AverageDurationSpentCodingInCurrentWeekInHms(totalTimeSpentCodingInCurrentWeek, totalNumOfSessionsInCurrentWeek);

        var rows = new List<Text>
        {
            new($"Total overall number of coding sessions: \t\t{totalNumOfSessions}"),
            new($"\nTotal overall duration spent coding: \t\t\t{totalTimeSpentCodingInHoursMinutesSeconds}"),
            new($"\nAverage time spent coding per session: \t\t\t{averageDurationInHoursMinutesSeconds}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current year: \t{totalNumOfSessionsInCurrentYear}"),
            new(
                $"\nTotal duration spent coding in the current year: \t{totalTimeSpentCodingInCurrentYearInHoursMinutesSeconds}"),
            new($"\nAverage time spent coding in the current year: \t\t{averageDurationSpentCodingInCurrentYearInHms}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current month: \t{totalNumOfSessionsInCurrentMonth}"),
            new($"\nTotal duration spent coding in the current month: \t{totalTimeSpentCodingInCurrentMonthInHms}"),
            new($"\nAverage time spent coding in the current month: \t{averageDurationSpentCodingInCurrentMonthInHms}"),
            new("\n"),
            new($"\nTotal number of coding sessions in the current week: \t{totalNumOfSessionsInCurrentWeek}"),
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
    
    public static void DisplayBreakdownByMonthForCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Now.Year;
        var occurrences =
            connection.Query<MonthlyOccurrence>(
                    Query.Session.GetTotalNumOfCodingSessionsForEachMonthOfTheCurrentYear, 
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
        
        var items = new List<(string Label, long Value, Color Color)>();

        foreach (var occurrence in occurrences)
        {
            var tuple = monthsDictionary[occurrence.Month];
            (string monthName, var color) = tuple;
            items.Add((monthName, occurrence.Occurrences, color));
        }

        AnsiConsole.Write(new BarChart()
            .Width(100)
            .Label($"[green bold underline]Monthly coding sessions breakdown for {currentYear}[/]")
            .CenterLabel()
            .AddItems(items, item => new BarChartItem(
                item.Label, item.Value, item.Color)));
    }
}