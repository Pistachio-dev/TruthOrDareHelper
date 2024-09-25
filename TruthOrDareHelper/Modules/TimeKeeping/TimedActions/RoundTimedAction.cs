using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TruthOrDareHelper.Modules.TimeKeeping.TimedActions
{
    public class RoundTimedAction : TimedAction
    {
        private int startRound;
        private int durationInRounds;
        private int currentRound;

        public void UpdateRoundCounter(int newValue)
        {
            currentRound = newValue;
        }

        public override bool HasElapsed()
        {
            // Note: These actions are often assigned at round end, so it's best to have them end at the beginning of startRound + durationInRounds + 1;
            return currentRound >= startRound + durationInRounds + 1;
        }

        public override void OnElapsed()
        {
            throw new NotImplementedException();
        }
    }
}
