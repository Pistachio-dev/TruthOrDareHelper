using Dalamud.Plugin.Services;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Modules.TimeKeeping.Interface
{
    public interface ITimeKeeper
    {
        void AddTimedAction(TimedAction action);
        void AttachToGameLogicLoop(IFramework frameworK);
    }
}