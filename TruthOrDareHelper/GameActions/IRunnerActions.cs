using Model;

namespace TruthOrDareHelper.GameActions
{
    public interface IRunnerActions
    {
        void CreateAndStartTimer(PlayerInfo target, string description, int minutes, int seconds);
        void CreateAndStartTimer(PlayerInfo target, string description, int roundAmount);
        void ReRoll(PlayerPair pair, bool rerollTheLoser);
        void Roll();
    }
}