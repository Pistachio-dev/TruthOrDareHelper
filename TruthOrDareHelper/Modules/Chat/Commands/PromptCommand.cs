using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.GameActions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class PromptCommand : ChatCommandBase
    {
        private readonly IRunnerActions runnerActions;

        public PromptCommand(IRunnerActions runnerActions, ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService)
        {
            this.runnerActions = runnerActions;
        }

        protected override bool IsMatch(string message)
        {
            return IsMatchAsSeparateWordInPhrase("prompt", message) || IsMatchAsSeparateWordInPhrase("hint", message);
        }

        protected override bool IsApplicable(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Winner.FullName == sender);
            if (relevantPlayingPair == null) { return false; }
            return !relevantPlayingPair.Done;
        }

        protected override void Execute(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Winner.FullName == sender);
            if (relevantPlayingPair == null) { return; }
            runnerActions.WritePrompt(relevantPlayingPair.Winner, relevantPlayingPair.ChallengeType);              
        }
    }
}
