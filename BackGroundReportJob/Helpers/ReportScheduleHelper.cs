using BackGroundReportJob.Models;

namespace BackGroundReportJob.Helpers
{
    public static class ReportScheduleHelper
    {
        public static bool ShouldGenerateReport(ReportConfigurationEntity report)
        {
            if (!report.IsEnabled)
                return false;

            var now = DateTime.UtcNow;

            return report.Frequency switch
            {
                ReportFrequency.Daily => IsTimeToRunDaily(now),
                ReportFrequency.Weekly => IsTimeToRunWeekly(now),
                ReportFrequency.Monthly => IsTimeToRunMonthly(now),
                ReportFrequency.Yearly => IsTimeToRunYearly(now),
                _ => false
            };
        }

        private static bool IsTimeToRunDaily(DateTime now)
        {
            // Run at midnight (00:00)
            //return now.Hour == 0 && now.Minute == 0;
            // Run every 1 minutes
            //return now.Minute % 1 == 0;
            return true;
        }

        private static bool IsTimeToRunWeekly(DateTime now)
        {
            // Run on Monday at midnight
            return now.DayOfWeek == DayOfWeek.Monday && IsTimeToRunDaily(now);
        }

        private static bool IsTimeToRunMonthly(DateTime now)
        {
            // Run on the 1st of each month at midnight
            return now.Day == 1 && IsTimeToRunDaily(now);
        }

        private static bool IsTimeToRunYearly(DateTime now)
        {
            // Run on January 1st at midnight
            return now.Month == 1 && now.Day == 1 && IsTimeToRunDaily(now);
        }
    }
}