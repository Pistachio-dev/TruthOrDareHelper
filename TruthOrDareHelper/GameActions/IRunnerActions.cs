using Model;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.GameActions
{
    public interface IRunnerActions
    {
        void ChatSoundWakeUp(PlayerInfo player);
        void CompletePair(PlayerPair pair);
        void CreateAndStartTimer(PlayerInfo target, string description, int minutes, int seconds);

        void CreateAndStartTimer(PlayerInfo target, string description, int roundAmount);
        void EndGame();
        void OpenPromptsFolder();
        void PrintChatCommands();
        void ReloadPrompts();
        void RemovePlayer(PlayerInfo player);
        void RemoveTimer(TimedAction timer);
        void ReRoll(PlayerPair pair, bool rerollTheLoser);
        void ReaddPromptFiles();
        void Roll();

        void TellWakeUp(PlayerInfo player);
        void ToggleAFK(PlayerInfo player);
        void WritePrompt(PlayerInfo player, ChallengeType challengeType, SafetyType? safetyType);
    }
}
