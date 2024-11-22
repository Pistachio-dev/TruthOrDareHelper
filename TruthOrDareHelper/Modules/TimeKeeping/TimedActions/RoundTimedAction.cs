using Model;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class RoundTimedAction : TimedAction
    {
        private int startRound;
        public  int DurationInRounds;
        public int Remaining => (startRound + DurationInRounds) - currentRound;

        private int currentRound;

        public RoundTimedAction(int startRound, int durationInRounds, PlayerInfo target, string description, OnTimedActionElapsed action)
            : base(target, description, action)
        {
            this.startRound = startRound;
            this.DurationInRounds = durationInRounds;
        }

        public override bool HasElapsed()
        {
            // Note: These actions are often assigned at round end, so it's best to have them end at the beginning of startRound + durationInRounds + 1;
            return currentRound >= startRound + DurationInRounds + 1;
        }

        public override void Update(ITruthOrDareSession session)
        {
            UpdateRoundCounter(session.Round);
        }

        private void UpdateRoundCounter(int newValue)
        {
            currentRound = newValue;
        }
    }
}
