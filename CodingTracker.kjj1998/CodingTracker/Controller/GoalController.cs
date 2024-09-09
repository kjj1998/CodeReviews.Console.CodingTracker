using CodingTracker.Model;
using CodingTracker.Repository;
using CodingTracker.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using Helper = CodingTracker.Utils.Helper;

namespace CodingTracker.Controller;

public static class GoalController
{
    public static void SetCodingGoals(SqliteConnection connection)
    {
        List<string> options =
        [
            "1. View progress", "2. Create goal", "3. Update goal", "4. Delete goal"
        ];

        const string instruction = "\n[bold]What would you like to [green]do[/][/]?";
        var highlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold);
        char option = Prompts.OptionSelectionPrompt(options, instruction, highlightStyle).ToCharArray()[0];

        switch (option)
        {
            case '1':
                ViewAllGoals(connection);
                break;
            case '2':
                InsertGoal(connection);
                break;
            case '3':
                UpdateGoal(connection);
                break;
            case '4':
                DeleteGoal(connection);
                break;
        }
        
        Helper.UserAcknowledgement();
    }
    
    private static void ViewAllGoals(SqliteConnection connection)
    {
        var goals = GoalRepo.GetGoals(connection);
        GoalRepo.DisplayGoals(goals, connection);
    }

    private static void InsertGoal(SqliteConnection connection)
    {
        string instruction = "Enter your [bold green]targeted hours[/] for coding";
        double hours = Prompts.DoubleValuePrompt(instruction);

        List<string> options = ["Daily", "Weekly", "Monthly"];
        instruction = "\n[bold]What kind of [green]goal[/] is this?[/]";
        var highlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold);
        string type = Prompts.OptionSelectionPrompt(options, instruction, highlightStyle);
        
        var goal = new Goal() { NumberOfHours = hours, Type = type};
        int rowsAffected = connection.Execute(Query.Goal.InsertGoal, goal);

        if (rowsAffected == 1)
            AnsiConsole.MarkupLine("\n[steelblue1 bold]Goal successfully added![/]");
    }
    
    private static void DeleteGoal(SqliteConnection connection)
    {
        var goals = GoalRepo.GetGoals(connection);

        if (goals.Count == 0)
        {
            Console.WriteLine("You don't have any goals!");
        }
        else
        {
            GoalRepo.DisplayGoals(goals, connection);
            HashSet<long> goalIds = [];
            foreach (var goal in goals)
            {
                goalIds.Add(goal.Id);
            }

            long selectedGoal = Prompts.GoalSelectionPrompt(goalIds, "delete");
            var deletedGoal = new Goal() { Id = selectedGoal };
            int rowsAffected = connection.Execute(Query.Goal.DeleteGoal, deletedGoal);
            
            if (rowsAffected == 1)
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Goal with id {selectedGoal} successfully deleted![/]");
        }
    }

    private static void UpdateGoal(SqliteConnection connection)
    {
        var goals = GoalRepo.GetGoals(connection);

        if (goals.Count == 0)
        {
            Console.WriteLine("You don't have any goals!");
        }
        else
        {
            GoalRepo.DisplayGoals(goals, connection);
            HashSet<long> goalIds = [];
            foreach (var goal in goals)
            {
                goalIds.Add(goal.Id);
            }

            long selectedGoal = Prompts.GoalSelectionPrompt(goalIds, "delete");
            AnsiConsole.MarkupLine(
                $"You have selected to edit the goal with id [aqua bold]{selectedGoal}[/]");
            string instruction = "Enter your [bold green]new targeted hours[/] for coding";
            double hours = Prompts.DoubleValuePrompt(instruction);

            List<string> options = ["Daily", "Weekly", "Monthly"];
            instruction = "\n[bold]Enter your [green]new type of goal[/][/]";
            var highlightStyle = new Style(Color.Yellow, Color.Blue1, Decoration.Bold);
            string type = Prompts.OptionSelectionPrompt(options, instruction, highlightStyle);

            var updatedGoal = new Goal() { Id = selectedGoal, Type = type, NumberOfHours = hours };

            int rowsAffected = connection.Execute(Query.Goal.UpdateGoal, updatedGoal);
            
            if (rowsAffected == 1)
                AnsiConsole.MarkupLine($"\n[steelblue1 bold]Goal with id {selectedGoal} successfully updated![/]");
        }
    }
}