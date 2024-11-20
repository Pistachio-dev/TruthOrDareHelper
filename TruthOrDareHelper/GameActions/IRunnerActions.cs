using Model;

namespace TruthOrDareHelper.GameActions
{
    public interface IRunnerActions
    {
        void ReRoll(PlayerPair pair, bool rerollTheLoser);
        void Roll();
    }
}