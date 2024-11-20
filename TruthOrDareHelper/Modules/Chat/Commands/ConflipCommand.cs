using DalamudBasics.Chat.Output;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using Model;
using System;
using System.Linq;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal class ConflipCommand : ChatCommandBase
    {
        public ConflipCommand(ITruthOrDareSession session, Configuration configuration, IChatOutput chatOutput, ILogService logService)
            : base(session, configuration, chatOutput, logService) { }

        protected override bool IsMatch(string message)
        {
            return IsMatchAsSeparateWordInPhrase("coin", message) || IsMatchAsSeparateWordInPhrase("coinflip", message);
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
            relevantPair.ChallengeType = FlipCoin();
            string resultText = GetCoinflipResult(relevantPair.ChallengeType);
            chatOutput.WriteChat($"{Plugin.MessageMark} {relevantPair.Winner.FullName.GetFirstName()} flips a coin and {resultText}");            
        }

        private ChallengeType FlipCoin()
        {
            int coinflip = new Random().Next(100);
            if (coinflip < 50)
            {
                return ChallengeType.Truth;
            }
            if (coinflip > 50)
            {
                return ChallengeType.Dare;
            }

            return ChallengeType.DealersChoice;
        }

        private string GetCoinflipResult(ChallengeType challengeType)
        {
            return challengeType switch
            {
                ChallengeType.Dare => "lands on Dare!",
                ChallengeType.Truth => "lands on Truth!",
                _ => "lands on it's side. What the hell. Dealer's choice then, I guess..."
            };
        }

    }
}
