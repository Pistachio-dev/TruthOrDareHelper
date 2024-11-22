using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.GameActions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class PasswordCommand : ChatCommandBase
    {
        private readonly IRunnerActions runnerActions;

        public PasswordCommand(ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService, IRunnerActions runnerActions)
            : base(session, configuration, chatOutput, logService)
        {
            this.runnerActions = runnerActions;
        }

        protected override bool IsMatch(string message)
        {
            return IsMatchWithNoOtherWords(configuration.ConfirmationKeyword, message);
        }

        protected override bool IsApplicable(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Winner.FullName == sender);
            if (relevantPlayingPair == null) { return false; }
            return !relevantPlayingPair.Done;
        }

        protected override void Execute(string sender, string message)
        {
            var relevantPair = session.PlayingPairs.First(pp => pp.Winner.FullName == sender);
            relevantPair.Done = true;
            chatOutput.WriteChat($"{relevantPair.Winner.FullName.GetFirstName()} accepts the {ChallengeTypeText(relevantPair.ChallengeType)}!");
            runnerActions.CompletePair(relevantPair);
        }

        private string ChallengeTypeText(ChallengeType challengeType)
        {
            return challengeType switch
            {
                ChallengeType.None => "Truth/Dare",
                ChallengeType.Truth => "Truth",
                ChallengeType.Dare => "Dare",
                ChallengeType.DealersChoice => "Truth/Dare",
                _ => "Truth/Dare",
            };
        }
    }
}
