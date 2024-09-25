using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Modules.TimeKeeping
{
    public class TimeKeeper
    {
        private int currentRound = 0;
        private LinkedList<TimedAction> timedActions = new();

        public void AddTimedAction(TimedAction action)
        {
            timedActions.AddLast(action);
        }

        public void Tick(int updatedCurrentRound)
        {
            currentRound = updatedCurrentRound;
            List<TimedAction> nodesToRemove = new();
            foreach (TimedAction action in timedActions)
            {
                if (action is RoundTimedAction roundTimedAction)
                {
                    roundTimedAction.UpdateRoundCounter(currentRound);
                }
                if (action.HasElapsed())
                {
                    try
                    {
                        Plugin.Log.Debug($"Executing action with ID {action.Id}");
                        action.OnElapsed();
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.Error(ex, $"Timed action with Id {action.Id} failed.");
                    }
                    finally
                    {
                        nodesToRemove.Add(action);
                        Plugin.Log.Debug($"Removed timed action with ID {action.Id}");
                    }                    
                }
            }

            foreach (TimedAction action in nodesToRemove)
            {
                timedActions.Remove(action);
            }
        }
    }
}
