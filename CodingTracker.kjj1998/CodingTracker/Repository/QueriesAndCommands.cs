namespace CodingTracker.Repository;

public static class QueriesAndCommands
{
    public const string CheckTableExists = """
                                           SELECT name 
                                           FROM sqlite_master 
                                           WHERE type='table' AND name=@tableName;
                                           """;

    public const string ViewAllRecords = """
                                         SELECT id, startTime, endTime, duration 
                                         FROM coding_sessions;
                                         """;

    public const string GetTotalTimeSpentCoding = """
                                                  SELECT SUM(duration) AS totalDuration 
                                                  FROM coding_sessions;
                                                  """;

    public const string GetTotalNumOfCodingSessions = "SELECT COUNT(Id) AS totalCount FROM coding_sessions;";

    public const string GetTotalTimeSpentCodingWithinATimePeriod = """
                                                                   SELECT SUM(duration) AS totalDuration 
                                                                   FROM coding_sessions 
                                                                   WHERE startTime>=@StartTime AND endTime<=@EndTime;
                                                                   """;

    public const string GetTotalNumOfCodingSessionsWithinATimePeriod = """
                                                                       SELECT COUNT(Id) AS totalCount 
                                                                       FROM coding_sessions 
                                                                       WHERE startTime>=@StartTime AND endTime<=@EndTime;
                                                                       """;

    public const string GetTotalNumOfCodingSessionsForEachMonthOfTheCurrentYear = """
                                                                                  WITH all_months AS (
                                                                                      SELECT 1 AS month
                                                                                      UNION ALL SELECT 2
                                                                                      UNION ALL SELECT 3
                                                                                      UNION ALL SELECT 4
                                                                                      UNION ALL SELECT 5
                                                                                      UNION ALL SELECT 6
                                                                                      UNION ALL SELECT 7
                                                                                      UNION ALL SELECT 8
                                                                                      UNION ALL SELECT 9
                                                                                      UNION ALL SELECT 10
                                                                                      UNION ALL SELECT 11
                                                                                      UNION ALL SELECT 12
                                                                                  )
                                                                                  SELECT
                                                                                      all_months.month AS Month,
                                                                                      COALESCE(COUNT(coding_sessions.startTime), 0) AS Occurrences
                                                                                  FROM
                                                                                      all_months
                                                                                  LEFT JOIN
                                                                                      coding_sessions
                                                                                  ON
                                                                                      CAST(strftime('%m', coding_sessions.startTime) AS INTEGER) = all_months.month
                                                                                  AND
                                                                                      strftime('%Y', coding_sessions.startTime) = @Year
                                                                                  GROUP BY
                                                                                      all_months.month
                                                                                  ORDER BY
                                                                                      all_months.month;
                                                                                  """;

    public const string InsertRecord = "INSERT INTO coding_sessions (startTime, endTime, duration) VALUES (@StartTime, @EndTime, @Duration)";

    public const string UpdateRecord = "UPDATE coding_sessions SET startTime = @StartTime, endTime = @EndTime, duration = @Duration WHERE id = @Id";

    public const string DeleteRecord = "DELETE FROM coding_sessions WHERE id = @Id";
    public const string CreateTable = "CREATE TABLE coding_sessions (id INTEGER PRIMARY KEY, startTime DATE NOT NULL, endTime DATE NOT NULL, duration INT NOT NULL)";
}