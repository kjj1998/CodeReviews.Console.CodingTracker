using CodingTracker.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace CodingTracker.Repository;

public static class GoalRepo
{
    public static void DisplayGoals(List<Goal> goals, SqliteConnection connection)
    {
        AnsiConsole.MarkupLine("\n[aqua bold underline]Goal Progress Summary[/]");

        foreach (var goal in goals)
        {
            double goalHours = goal.NumberOfHours;
            string? goalType = goal.Type;
            long goalId = goal.Id;

            (int timeSpent, int amtTimeToCodeToCompleteGoal) = CheckGoalCompletion(connection, goalHours, goalType);
            string status = amtTimeToCodeToCompleteGoal == 0 ? 
                "[green bold]Completed[/]" : 
                "[red bold]Incomplete[/]";
            
            string timeToCompleteGoalInHms = Utils.Helper.ConvertSecondsToHoursMinutesSeconds(amtTimeToCodeToCompleteGoal);
            string timeSpentInHms = Utils.Helper.ConvertSecondsToHoursMinutesSeconds(timeSpent);
            
            AnsiConsole.MarkupLine($"Id: {goalId}, Goal: [aqua]code {goalHours} hours {goalType?.ToLower()}[/], " +
                                   $"Time spent coding so far: [aqua]{timeSpentInHms}[/], Status: {status}, " +
                                   $"Daily coding target to achieve goal: [bold yellow]{timeToCompleteGoalInHms}[/]");
        }
    }
    
    public static List<Goal> GetGoals(SqliteConnection connection)
    {
        var goals = connection.Query<Goal>(Query.Goal.GetAllGoals).ToList();

        return goals;
    }

    public static (int, int) CheckGoalCompletion(SqliteConnection connection, double goalHours, string? goalType)
    {
        int target = Helper.ConvertHoursToSeconds(goalHours);
        int timeSpent = 0;
        int amtOfTimeToCodeDailyToCompleteGoal = 0;
        
        switch (goalType)
        {
            case "Daily":
                timeSpent = DatabaseAccess.GetTotalTimeSpentCodingInTheCurrentDay(connection);
                if (timeSpent < target)
                    amtOfTimeToCodeDailyToCompleteGoal = target - timeSpent;
                break;
            case "Monthly":
                timeSpent = DatabaseAccess.GetTotalTimeSpentCodingInTheCurrentMonth(connection);
                if (timeSpent < target)
                    amtOfTimeToCodeDailyToCompleteGoal = 
                        Helper.DetermineAmountOfCodingToCompleteGoal(target - timeSpent, "monthly");
                break;
            case "Weekly":
                timeSpent = DatabaseAccess.GetTotalTimeSpentCodingInTheCurrentWeek(connection);
                if (timeSpent < target)
                    amtOfTimeToCodeDailyToCompleteGoal = 
                        Helper.DetermineAmountOfCodingToCompleteGoal(target - timeSpent, "weekly");
                break;
        }

        return (timeSpent, amtOfTimeToCodeDailyToCompleteGoal);
    }
}