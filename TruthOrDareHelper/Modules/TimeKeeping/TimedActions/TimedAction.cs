using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public abstract class TimedAction
    {
        public delegate void TimeEndActionDelegate();

        public Guid Id { get; set; } = Guid.NewGuid();
        
        protected TimeEndActionDelegate timeEndAction;
        protected DateTime StartTime { get; set; } = DateTime.Now; // Useful for logging, even if it's not really used on the RoundTimedAction
        

        public abstract bool HasElapsed();

        public void OnElapsed()
        {
            timeEndAction();
        }
        


    }
}
