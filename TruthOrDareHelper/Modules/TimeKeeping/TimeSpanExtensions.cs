using System;

namespace TruthOrDareHelper.Modules.TimeKeeping
{
    public static class TimeSpanExtensions
    {
        public static string ToShortString(this TimeSpan time)
        {
            return new TimeSpan(time.Hours, time.Minutes, time.Seconds).ToString("c");
        }
    }
}
