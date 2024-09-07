namespace CodingTracker.Models;

public class MonthlyOccurrence
{
    public string? Year { get; init; }
    public int Month { get; init; }
    public long Occurrences { get; init; }
}