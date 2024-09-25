using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public abstract class TimedAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        protected DateTime StartTime { get; set; } // Useful for logging, even if it's not really used on the RoundTimedAction

        public abstract bool HasElapsed();
        public abstract void OnElapsed();
    }
}
