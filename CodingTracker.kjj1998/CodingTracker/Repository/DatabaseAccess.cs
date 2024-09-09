using CodingTracker.Model;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Repository;

public static class DatabaseAccess
{
    public static List<Session> GetAllSessions(SqliteConnection connection, string query)
    {
        return connection.Query<Session>(query).ToList();
    }
    
    private static List<Session> GetAllSessionsWithinATimePeriod(
        SqliteConnection connection,
        string query,
        DateTime start, 
        DateTime end)
    {
        var condition = new Session() { StartTime = start, EndTime = end };

        return connection.Query<Session>(query, condition).ToList();
    }
    
    public static List<Session> GetAllSessionsInCurrentYear(SqliteConnection connection, string query)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);

        return GetAllSessionsWithinATimePeriod(connection, query, start, end);
    }
    
    public static List<Session> GetAllSessionsInCurrentMonth(SqliteConnection connection, string query)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        return GetAllSessionsWithinATimePeriod(connection, query, firstDayOfMonth, lastDayOfMonth);
    }
    
    public static List<Session> GetAllSessionsInCurrentWeek(SqliteConnection connection, string query)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetAllSessionsWithinATimePeriod(connection, query, startOfWeek, endOfWeek);
    }
    
    public static int GetTotalNumOfSessions(SqliteConnection connection)
    {
        return connection.ExecuteScalar<int>(Query.Session.GetTotalNumOfSessions);
    }
    
    private static int GetTotalNumOfSessionsWithinATimePeriod(SqliteConnection connection, DateTime start, DateTime end)
    {
        var condition = new Session() { StartTime = start, EndTime = end };
        
        return connection.ExecuteScalar<int>(Query.Session.GetTotalNumOfSessionsWithinATimePeriod, condition);
    }
    
    public static int GetTotalNumOfSessionsInTheCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);
        
        return GetTotalNumOfSessionsWithinATimePeriod(connection, start, end);
    }
    
    public static int GetTotalNumOfSessionsInTheCurrentMonth(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return GetTotalNumOfSessionsWithinATimePeriod(connection, firstDayOfMonth, lastDayOfMonth);
    }
    
    public static int GetTotalNumOfSessionsInTheCurrentWeek(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetTotalNumOfSessionsWithinATimePeriod(connection, startOfWeek, endOfWeek);
    }
    
    public static int GetTotalTimeSpentCoding(SqliteConnection connection)
    {
        return connection.ExecuteScalar<int>(Query.Session.GetTotalTimeSpentCoding);
    }
    
    private static int GetTotalTimeSpentCodingWithinATimePeriod(SqliteConnection connection, DateTime start, DateTime end)
    {
        var condition = new Session() { StartTime = start, EndTime = end };

        return connection.ExecuteScalar<int>(Query.Session.GetTotalTimeSpentCodingWithinATimePeriod, condition);
    }
    
    public static int GetTotalTimeSpentCodingInTheCurrentYear(SqliteConnection connection)
    {
        int currentYear = DateTime.Today.Year;
        var start = new DateTime(currentYear, 1, 1);
        var end = new DateTime(currentYear, 12, 31);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, start, end);
    }
    
    public static int GetTotalTimeSpentCodingInTheCurrentMonth(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, firstDayOfMonth, lastDayOfMonth);
    }
    
    public static int GetTotalTimeSpentCodingInTheCurrentWeek(SqliteConnection connection)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = today.DayOfWeek;
        var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(6);

        return GetTotalTimeSpentCodingWithinATimePeriod(connection, startOfWeek, endOfWeek);
    }

    public static int GetTotalTimeSpentCodingInTheCurrentDay(SqliteConnection connection)
    {
        var today = DateTime.Today;
        
        return GetTotalTimeSpentCodingWithinATimePeriod(connection, today, today);
    }
    
}