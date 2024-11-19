using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using TruthOrDareHelper.Modules.TimeKeeping.Interface;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Modules.TimeKeeping
{
    public class TimeKeeper : ITimeKeeper
    {
        private int currentRound = 0;
        private LinkedList<TimedAction> timedActions = new();
        private ILogService log;

        public TimeKeeper()
        {
            log = Plugin.Resolve<ILogService>();
        }

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
                        log.Debug($"Executing action with ID {action.Id}");
                        action.OnElapsed();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, $"Timed action with Id {action.Id} failed.");
                    }
                    finally
                    {
                        nodesToRemove.Add(action);
                        log.Debug($"Removed timed action with ID {action.Id}");
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
