using Model;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class RoundTimedAction : TimedAction
    {
        private int startRound;
        private int durationInRounds;
        private int currentRound;

        public RoundTimedAction(int startRound, int durationInRounds, TimeEndActionDelegate action)
        {
            this.startRound = startRound;
            this.durationInRounds = durationInRounds;
            this.timeEndAction = action;
        }

        public override bool HasElapsed()
        {
            // Note: These actions are often assigned at round end, so it's best to have them end at the beginning of startRound + durationInRounds + 1;
            return currentRound >= startRound + durationInRounds + 1;
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
