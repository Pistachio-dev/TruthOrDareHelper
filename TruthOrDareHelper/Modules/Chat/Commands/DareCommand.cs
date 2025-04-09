using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class DareCommand : ChatCommandBase
    {
        public DareCommand(ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService) { }

        protected override bool IsMatch(string message)
        {
            return IsMatchWithNoOtherWords("d", message) || IsMatchAsSeparateWordInPhrase("dare", message);
        }

        protected override bool IsApplicable(string sender)
        {
            var relevantPlayingPair = session.PlayingPairs.FirstOrDefault(pp => pp.Loser?.FullName == sender);
            if (relevantPlayingPair == null) { return false; }
            return !relevantPlayingPair.Done;
        }

        protected override void Execute(string sender, string message)
        {            
            var relevantPair = session.PlayingPairs.First(pp => pp.Loser?.FullName == sender);
            if (IsForbiddenReChoice(relevantPair)){
                logService.Info($"Rechoice of dare for player {relevantPair.Loser.FullName} blocked.");
                return;
            }
            relevantPair.ChallengeType = ChallengeType.Dare;
            if (configuration.ConfirmChallengeChoice)
            {            
                chatOutput.WriteChat($"{relevantPair.Loser?.FullName.GetFirstName()} chooses Dare!");
            }
        }
    }
}
