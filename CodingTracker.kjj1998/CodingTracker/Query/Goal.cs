namespace CodingTracker.Query;

public static class Goal
{
    public const string InsertGoal = "INSERT INTO goals (numberOfHours, type) VALUES (@NumberOfHours, @Type)";
    
    public const string UpdateGoal = "UPDATE goals SET numberOfHours = @NumberOfHours, type = @Type WHERE id = @Id";
    
    public const string DeleteGoal = "DELETE FROM goals WHERE id = @Id";
    
    public const string CreateGoalsTable =
        "CREATE TABLE goals (id INTEGER PRIMARY KEY, numberOfHours DOUBLE NOT NULL, type TEXT NOT NULL)";
    
    public const string GetAllGoals = "SELECT id, numberOfHours, type FROM goals";
}