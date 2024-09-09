namespace CodingTracker.Repository;

public static class Helper
{
    public static string AverageDurationSpentCodingInCurrentWeekInHms(int totalTimeSpentCodingInCurrentWeek,
        int totalNumOfCodingSessionsInCurrentWeek)
    {
        int averageDurationSpentCodingInCurrentWeek =
            (int)Math.Round(totalTimeSpentCodingInCurrentWeek / (double)totalNumOfCodingSessionsInCurrentWeek);
        string averageDurationSpentCodingInCurrentWeekInHms =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentWeek);
        return averageDurationSpentCodingInCurrentWeekInHms;
    }

    public static string AverageDurationSpentCodingInCurrentMonthInHms(int totalTimeSpentCodingInCurrentMonth,
        int totalNumOfCodingSessionsInCurrentMonth)
    {
        int averageDurationSpentCodingInCurrentMonth
            = (int)Math.Round(totalTimeSpentCodingInCurrentMonth / (double)totalNumOfCodingSessionsInCurrentMonth);
        string averageDurationSpentCodingInCurrentMonthInHms =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentMonth);
        return averageDurationSpentCodingInCurrentMonthInHms;
    }

    public static string AverageDurationSpentCodingInCurrentYearInHms(int totalTimeSpentCodingInCurrentYear,
        int totalNumOfCodingSessionsInCurrentYear)
    {
        int averageDurationSpentCodingInCurrentYear =
            (int)Math.Round(totalTimeSpentCodingInCurrentYear / (double)totalNumOfCodingSessionsInCurrentYear);
        string averageDurationSpentCodingInCurrentYearInHms =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(averageDurationSpentCodingInCurrentYear);
        return averageDurationSpentCodingInCurrentYearInHms;
    }

    public static string AverageDurationInHoursMinutesSeconds(int totalTimeSpentCodingInSeconds,
        int totalNumOfCodingSessions)
    {
        int averageDuration = (int)Math.Round(totalTimeSpentCodingInSeconds / (double)totalNumOfCodingSessions);
        string averageDurationInHoursMinutesSeconds =
            Utils.Helper.ConvertSecondsToHoursMinutesSeconds(averageDuration);
        return averageDurationInHoursMinutesSeconds;
    }
    public static int ConvertHoursToSeconds(double hours)
    {
        double seconds = Math.Round(hours * 3600);

        return (int)seconds;
    }

    public static int DetermineAmountOfCodingToCompleteGoal(int remaining, string type)
    {
        int durationToCodePerDayToCompleteGoal;
        
        if (type.Equals("monthly"))
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            int numberOfDaysLeft = lastDayOfMonth.Day - today.Day;

            durationToCodePerDayToCompleteGoal = (int) Math.Round(remaining / (double)numberOfDaysLeft);
        }
        else
        {
            var today = DateTime.Today;
            var currentDayOfWeek = today.DayOfWeek;
            var startOfWeek = today.AddDays(-((int)currentDayOfWeek + 6) % 7);
            var endOfWeek = startOfWeek.AddDays(6);
            int numberOfDaysLeft = endOfWeek.Day - today.Day;
            
            durationToCodePerDayToCompleteGoal = (int) Math.Round(remaining / (double)numberOfDaysLeft);
        }

        return durationToCodePerDayToCompleteGoal;
    }
}