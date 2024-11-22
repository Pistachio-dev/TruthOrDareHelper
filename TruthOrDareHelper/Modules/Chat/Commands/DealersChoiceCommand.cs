using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class DealersChoiceCommand : ChatCommandBase
    {
        public DealersChoiceCommand(ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService) { }

        protected override bool IsMatch(string message)
        {
            return IsMatchWithNoOtherWords("dch", message)
                || IsMatchAsSeparateWordInPhrase("dealer's choice", message)
                || IsMatchAsSeparateWordInPhrase("dealers choice", message)
                || IsMatchAsSeparateWordInPhrase("your choice", message)
                || IsMatchAsSeparateWordInPhrase("you decide", message);
        }

        protected override bool IsApplicable(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Loser?.FullName == sender);
            if (relevantPlayingPair == null) { return false; }
            return !relevantPlayingPair.Done;
        }

        protected override void Execute(string sender)
        {
            var relevantPair = session.PlayingPairs.First(pp => pp.Loser?.FullName == sender);
            relevantPair.ChallengeType = ChallengeType.DealersChoice;
            chatOutput.WriteChat($"{relevantPair.Loser?.FullName.GetFirstName()} lets {relevantPair.Winner.FullName.GetFirstName()} decide!");
        }
    }
}
