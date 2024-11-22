using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class TruthCommand : ChatCommandBase
    {
        public TruthCommand(ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService) { }

        protected override bool IsMatch(string message)
        {
            return IsMatchWithNoOtherWords("t", message) || IsMatchAsSeparateWordInPhrase("truth", message);
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
            if (IsForbiddenReChoice(relevantPair))
            {
                logService.Info($"Rechoice of truth for player {relevantPair.Loser.FullName} blocked.");
                return;
            }
            relevantPair.ChallengeType = ChallengeType.Truth;
            chatOutput.WriteChat($"{relevantPair.Loser?.FullName.GetFirstName()} chooses Truth!");
        }
    }
}
