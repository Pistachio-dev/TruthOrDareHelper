using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class TimerTimedAction : TimedAction
    {        
        private TimeSpan duration;

        public TimerTimedAction(TimeSpan duration, TimeEndActionDelegate action)
        {
            this.duration = duration;
            this.timeEndAction = action;
        }

        public override bool HasElapsed()
        {
            return DateTime.Now - StartTime >= duration;
        }        
    }
}
