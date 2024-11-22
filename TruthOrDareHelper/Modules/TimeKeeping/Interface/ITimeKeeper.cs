using Dalamud.Plugin.Services;
using Model;
using System.Collections.Generic;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Modules.TimeKeeping.Interface
{
    public interface ITimeKeeper
    {
        LinkedList<TimedAction> Timers { get; }

        void AddTimedAction(TimedAction action);

        void AttachToGameLogicLoop(IFramework frameworK);
        void RemoveTimedAction(TimedAction action);
        void RemoveTimersForPlayer(PlayerInfo playerInfo);
    }
}
