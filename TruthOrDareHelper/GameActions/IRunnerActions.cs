using Model;

namespace TruthOrDareHelper.GameActions
{
    public interface IRunnerActions
    {
        void ChatSoundWakeUp(PlayerInfo player);
        void CreateAndStartTimer(PlayerInfo target, string description, int minutes, int seconds);
        void CreateAndStartTimer(PlayerInfo target, string description, int roundAmount);
        void OpenPromptsFolder();
        void ReloadPrompts();
        void ReRoll(PlayerPair pair, bool rerollTheLoser);
        void Roll();
        void TellWakeUp(PlayerInfo player);
        void WritePrompt(PlayerInfo player, ChallengeType challengeType);
    }
}