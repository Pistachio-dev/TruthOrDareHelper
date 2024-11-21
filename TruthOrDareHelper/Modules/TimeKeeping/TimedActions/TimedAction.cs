using Model;
using System;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public abstract class TimedAction
    {
        public delegate void OnTimedActionElapsed();

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime StartTime { get; set; } = DateTime.Now; // Useful for logging, even if it's not really used on the RoundTimedAction

        public PlayerInfo Target { get; set; }
        public string Description { get; set; }

        protected OnTimedActionElapsed onElapsedAction;

        public TimedAction(PlayerInfo target, string description, OnTimedActionElapsed onElapsedAction)
        {
            Target = target;
            Description = description;
            this.onElapsedAction = onElapsedAction;
        }

        public abstract bool HasElapsed();
        public abstract void Update(ITruthOrDareSession session);

        public void OnElapsed()
        {
            onElapsedAction();
        }
    }
}
