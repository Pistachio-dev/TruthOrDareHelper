using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public abstract class TimedAction
    {
        public abstract bool HasCompleted();
        public abstract void OnElapsed();
    }
}
