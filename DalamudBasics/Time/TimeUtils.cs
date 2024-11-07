using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Time
{
    public class TimeUtils : ITimeUtils
    {
        private readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public long GetNowUnixTimestamp(bool useUtc = true)
        {
            DateTime now = useUtc ? DateTime.UtcNow : DateTime.Now;
            return (now - Epoch).Ticks;
        }

        public DateTime GetLocalDateTime()
        {
            return DateTime.Now;
        }
    }
}
