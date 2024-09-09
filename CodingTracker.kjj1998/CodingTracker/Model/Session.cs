namespace CodingTracker.Model;

public class Session()
{
    public long Id { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public long Duration { get; init; }
}