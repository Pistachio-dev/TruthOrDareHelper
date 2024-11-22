using Model;
using System;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class TimerTimedAction : TimedAction
    {
        public TimeSpan Duration;

        public TimeSpan Remaining => (StartTime + Duration) - DateTime.Now;

        public TimerTimedAction(TimeSpan duration, PlayerInfo target, string description, OnTimedActionElapsed action)
            : base(target, description, action)
        {
            this.Duration = duration;
        }

        public override bool HasElapsed()
        {
            return DateTime.Now - StartTime >= Duration;
        }

        public override void Update(ITruthOrDareSession session)
        {
            return;
        }
    }
}
