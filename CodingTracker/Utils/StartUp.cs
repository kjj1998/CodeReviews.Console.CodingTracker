using CodingTracker.Repository;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Utils;

public static class StartUp
{
    public static SqliteConnection SystemStartUpCheck()
    {
        string dbName =  Utils.Config.GetSection("Database:Name").Value ?? string.Empty;
        string dbPath = Utils.FindDirectoryOfFile(Directory.GetCurrentDirectory(), "Program.cs") + "/" + dbName;
        string tableName = Utils.Config.GetSection("Database:TableName").Value ?? string.Empty;
        
        Console.WriteLine(dbPath);
        
        Console.WriteLine("\nPerforming application start up checks...");
        var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        Console.WriteLine("Successfully connected to SQLite database!");

        if (!CheckTableExists(connection, tableName))
        {
            CreateTable(connection, tableName);
            SeedTable(connection);
        }
        else
        {
            Console.WriteLine($"{tableName} table found!");
        }

        return connection;
    }
    
    private static bool CheckTableExists(SqliteConnection connection, string tableName)
    {
        using var command = new SqliteCommand(QueriesAndCommands.CheckTableExists, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        using var reader = command.ExecuteReader();

        return reader.HasRows;
    }

    private static void CreateTable(SqliteConnection connection, string tableName)
    {
        using var command = new SqliteCommand(QueriesAndCommands.CreateTable, connection);
        command.Parameters.AddWithValue("@tableName", tableName);

        command.ExecuteNonQuery();
        Console.WriteLine($"{tableName} table successfully created.");
    }
    
    private static void SeedTable(SqliteConnection connection)
    {
        var today = DateTime.Now;
        int prevMonth = today.Month - 1;
        int prevYear = today.Year;
        var random = new Random();

        if (prevMonth < 1)
        {
            prevMonth = 12;
            prevYear -= 1;
        }

        var date = new DateTime(prevYear, prevMonth, 1);

        for (int i = 0; i < 28; i++)
        {
            int hour = random.Next(0, 24); // Random hour between 0 and 23
            int minute = random.Next(0, 60); // Random minute between 0 and 59
            int second = random.Next(0, 60); // Random second between 0 and 59
            int codingSessionHours = random.Next(1, 4);
            int codingSessionMinutes = random.Next(0, 60);
            int codingSessionSeconds = random.Next(0, 60);
            
            var startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
            var endTime = startTime.AddHours(codingSessionHours).AddMinutes(codingSessionMinutes).AddSeconds(codingSessionSeconds);
            int codingSessionDurationInSeconds = codingSessionHours * 3600 + codingSessionMinutes * 60 + codingSessionSeconds;

            using var command = new SqliteCommand(QueriesAndCommands.InsertRecord, connection);
            command.Parameters.AddWithValue("@startTime", startTime);
            command.Parameters.AddWithValue("@endTime", endTime);
            command.Parameters.AddWithValue("@duration", codingSessionDurationInSeconds);
            command.ExecuteNonQuery();
            
            date = date.AddDays(1);
        }
    }
}