using Model;
using System;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class TimerTimedAction : TimedAction
    {
        private TimeSpan duration;

        public TimerTimedAction(TimeSpan duration, PlayerInfo target, string description, OnTimedActionElapsed action)
            : base(target, description, action)
        {
            this.duration = duration;
        }

        public override bool HasElapsed()
        {
            return DateTime.Now - StartTime >= duration;
        }

        public override void Update(ITruthOrDareSession session)
        {
            return;
        }
    }
}
