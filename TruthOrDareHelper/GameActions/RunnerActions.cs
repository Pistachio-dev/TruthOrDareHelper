using DalamudBasics.Configuration;
using DalamudBasics.Targeting;
using Model;
using System.Collections.Generic;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.GameActions
{
    public class RunnerActions : IRunnerActions
    {
        private readonly ITruthOrDareSession session;
        private readonly IToDChatOutput chatOutput;
        private readonly IRollManager rollManager;
        private readonly ISignManager signManager;
        private readonly ITargetingService targetingManager;
        private readonly Configuration configuration;

        public RunnerActions(ITruthOrDareSession session, IConfigurationService<Configuration> configService, IToDChatOutput chatOutput, IRollManager rollManager, ISignManager signManager,
            ITargetingService targetingManager)
        {
            this.session = session;
            this.chatOutput = chatOutput;
            this.rollManager = rollManager;
            this.signManager = signManager;
            this.targetingManager = targetingManager;
            this.configuration = configService.GetConfiguration();
        }

        public void Roll()
        {
            signManager.ClearMarks(session.PlayingPairs);
            session.Round++;
            // TODO: Before next roll, make sure to add the "truth wins, dare wins, etc" stats if available
            List<PlayerPair> pairs = rollManager.RollStandard(session.PlayerInfo.Select(kvp => kvp.Value).ToList(), configuration.MaxParticipationStreak, configuration.SimultaneousPlays);
            foreach (var player in session.PlayerInfo.Select(p => p.Value))
            {
                if (pairs.FirstOrDefault(p => p.Winner == player) != null)
                {
                    player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.Winner));
                }
                else if (pairs.FirstOrDefault(p => p.Loser == player) != null)
                {
                    player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.Loser));
                }
                else
                {
                    player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.NotParticipating));
                }
            }

            session.PlayingPairs = pairs;
            signManager.ApplyMarks(session.PlayingPairs);

            string optionalS = pairs.Count > 1 ? "S" : string.Empty;
            chatOutput.WriteChat($"-------------ROLLING NEW COUPLE{optionalS}--------------");
            chatOutput.WritePairs(pairs);
        }

        

        public void ReRoll(PlayerPair pair, bool rerollTheLoser)
        {
            string rerrolledName = rerollTheLoser ? pair.Loser?.FullName ?? "Nobody? This should not be possible." : pair.Winner.FullName;
            chatOutput.WriteChat($"Rerolling {rerrolledName}.");
            PlayerInfo? replacement = rollManager.Reroll(session);
            if (replacement == null)
            {
                return;
            }

            if (rerollTheLoser && pair.Loser != null)
            {
                signManager.UnmarkPlayer(pair.Loser);
                pair.Loser = replacement;
                signManager.MarkPlayer(pair.Loser, false);
            }
            else
            {
                signManager.UnmarkPlayer(pair.Winner);
                pair.Winner = replacement;
                signManager.MarkPlayer(pair.Winner, true);
            }

            chatOutput.WriteChat($"Rerroll! {replacement.FullName} replaces {rerrolledName}.");
        }
    }
}
