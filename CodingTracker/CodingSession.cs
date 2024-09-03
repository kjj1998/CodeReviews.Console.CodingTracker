namespace CodingTracker;

public class CodingSession(int id, DateTime startTime, DateTime endTime, int duration)
{
    private int _id = id;
    private DateTime _startTime = startTime;
    private DateTime _endTime = endTime;
    private int _duration = duration;

    public static int CalculateDuration(DateTime startTime, DateTime endTime)
    {
        var difference = endTime - startTime;

        return difference.Seconds;
    }

    public static bool CheckEndTimeGreaterThanStartTime(DateTime startTime, DateTime endTime)
    {
        var difference = endTime - startTime;

        return difference.Seconds >= 0;
    }
}