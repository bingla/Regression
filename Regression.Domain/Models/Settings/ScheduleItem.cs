namespace Regression.Domain.Models.Settings
{
    public class ScheduleItem
    {
        public DayOfWeek? Day { get; set; }
        public TimeOnly Time { get; set; }
    }
}
