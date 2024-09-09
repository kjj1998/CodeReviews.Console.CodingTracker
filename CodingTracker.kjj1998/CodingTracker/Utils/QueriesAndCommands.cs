namespace CodingTracker.Repository;

public static class QueriesAndCommands
{
    public const string CheckTableExists = """
                                           SELECT name 
                                           FROM sqlite_master 
                                           WHERE type='table' AND name=@tableName;
                                           """;

    public const string InsertRecord = "INSERT INTO coding_sessions (startTime, endTime, duration) VALUES (@StartTime, @EndTime, @Duration)";

    public const string CreateTable = "CREATE TABLE coding_sessions (id INTEGER PRIMARY KEY, startTime DATE NOT NULL, endTime DATE NOT NULL, duration INT NOT NULL)";

    public const string CreateGoalsTable =
        "CREATE TABLE goals (id INTEGER PRIMARY KEY, numberOfHours DOUBLE NOT NULL, type TEXT NOT NULL)";
}