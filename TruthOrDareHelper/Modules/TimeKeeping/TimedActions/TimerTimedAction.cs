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
        public override bool HasElapsed()
        {
            return DateTime.Now - StartTime >= duration;
        }

        public override void OnElapsed()
        {
            throw new NotImplementedException();
        }
    }
}
