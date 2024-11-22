using Dalamud.Plugin.Services;
using DalamudBasics.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TruthOrDareHelper.Modules.TimeKeeping.Interface;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Modules.TimeKeeping
{
    public class TimeKeeper : ITimeKeeper
    {
        private int currentRound => session.Round;
        private LinkedList<TimedAction> timedActions = new();
        private readonly ITruthOrDareSession session;
        private ILogService logsService;
        private bool initialized = false;

        public LinkedList<TimedAction> Timers => timedActions;

        public TimeKeeper(ITruthOrDareSession session, ILogService logService)
        {
            this.session = session;
            logsService = logService;
        }

        public void AttachToGameLogicLoop(IFramework frameworK)
        {
            frameworK.Update += Tick;
            initialized = true;
            logsService.Info($"{nameof(TimeKeeper)} initialized.");
        }

        public void AddTimedAction(TimedAction action)
        {
            if (!initialized)
            {
                logsService.Error($"{nameof(TimeKeeper)} was not initialized!");
            }

            timedActions.AddLast(action);
        }

        public void RemoveTimedAction(TimedAction action)
        {
            timedActions.Remove(action);
        }

        public void RemoveTimersForPlayer(PlayerInfo playerInfo)
        {
            var toRemove = timedActions.Where(t => t.Target == playerInfo).ToList();
            foreach (var t in toRemove)
            {
                RemoveTimedAction(t);
            }
        }

        private void Tick(IFramework dalamudFramework)
        {
            List<TimedAction> nodesToRemove = new();
            foreach (TimedAction action in timedActions)
            {
                action.Update(session);
                if (action.HasElapsed())
                {
                    try
                    {
                        logsService.Debug($"Executing action with ID {action.Id}");
                        action.OnElapsed();
                    }
                    catch (Exception ex)
                    {
                        logsService.Error(ex, $"Timed action with Id {action.Id} failed.");
                    }
                    finally
                    {
                        nodesToRemove.Add(action);
                    }
                }
            }

            foreach (TimedAction action in nodesToRemove)
            {
                timedActions.Remove(action);
                logsService.Debug($"Removed timed action with ID {action.Id}");
            }
        }
    }
}
