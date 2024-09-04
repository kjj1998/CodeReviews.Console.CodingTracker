using System.Globalization;

namespace CodingTracker;

public class CodingSession(long id, string startTime, string endTime, long duration)
{
    public long Id = id;
    public string StartTime = startTime;
    public string EndTime = endTime;
    public long Duration = duration;

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