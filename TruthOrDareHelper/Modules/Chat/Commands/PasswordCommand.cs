using DalamudBasics.Chat.Output;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class PasswordCommand : ChatCommandBase
    {
        public PasswordCommand(ITruthOrDareSession session, Configuration configuration, IChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService) { }

        protected override bool IsMatch(string message)
        {
            return IsMatchAsSeparateWordInPhrase(configuration.ConfirmationKeyword, message);
        }

        protected override bool IsApplicable(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Winner.FullName == sender);
            if (relevantPlayingPair == null) { return false; }
            return !relevantPlayingPair.Done;
        }

        protected override void Execute(string sender)
        {
            var relevantPair = session.PlayingPairs.First(pp => pp.Winner.FullName == sender);
            relevantPair.Done = true;
            chatOutput.WriteChat($"{Plugin.MessageMark} {relevantPair.Winner.FullName.GetFirstName()} accepts the {ChallengeTypeText(relevantPair.ChallengeType)}!");            
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
