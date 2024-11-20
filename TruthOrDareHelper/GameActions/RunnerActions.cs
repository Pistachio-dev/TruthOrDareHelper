using DalamudBasics.Chat.Output;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;
using DalamudBasics.Configuration;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.GameActions
{
    public class RunnerActions : IRunnerActions
    {
        private readonly ITruthOrDareSession session;
        private readonly IToDChatOutput chatOutput;
        private readonly IRollManager rollManager;
        private readonly Configuration configuration;

        public RunnerActions(ITruthOrDareSession session, IConfigurationService<Configuration> configService, IToDChatOutput chatOutput, IRollManager rollManager)
        {
            this.session = session;
            this.chatOutput = chatOutput;
            this.rollManager = rollManager;
            this.configuration = configService.GetConfiguration();
        }

        public void Roll()
        {
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
            chatOutput.WriteChat($"-------------ROLLING--------------");
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

            if (rerollTheLoser)
            {
                pair.Loser = replacement;
            }
            else
            {
                pair.Winner = replacement;
            }

            chatOutput.WriteChat($"Rerroll! {replacement.FullName} replaces {rerrolledName}.");
        }
    }
}
