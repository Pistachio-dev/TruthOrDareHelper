using Dalamud.Utility;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Modules.TimeKeeping.Interface;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;
using TruthOrDareHelper.Settings;
using static TruthOrDareHelper.Modules.TimeKeeping.TimedActions.TimedAction;

namespace TruthOrDareHelper.GameActions
{
    public class RunnerActions : IRunnerActions
    {
        private readonly ITruthOrDareSession session;
        private readonly IToDChatOutput chatOutput;
        private readonly IRollManager rollManager;
        private readonly ISignManager signManager;
        private readonly ITargetingService targetingManager;
        private readonly ILogService log;
        private readonly IClientChatGui chatGui;
        private readonly ITimeKeeper timeKeeper;
        private readonly Configuration configuration;

        public RunnerActions(ITruthOrDareSession session, IConfigurationService<Configuration> configService, IToDChatOutput chatOutput, IRollManager rollManager, ISignManager signManager,
            ITargetingService targetingManager, ILogService log, IClientChatGui chatGui, ITimeKeeper timeKeeper)
        {
            this.session = session;
            this.chatOutput = chatOutput;
            this.rollManager = rollManager;
            this.signManager = signManager;
            this.targetingManager = targetingManager;
            this.log = log;
            this.chatGui = chatGui;
            this.timeKeeper = timeKeeper;
            this.configuration = configService.GetConfiguration();
        }

        public void Roll()
        {
            log.Info($"[ACTION] Create timer: Roll. Type: {configuration.RollingType}");
            signManager.ClearMarks(session.PlayingPairs);
            session.Round++;
            
            switch (configuration.RollingType)
            {
                case RollingType.PluginRng:
                    session.PlayingPairs = rollManager.RollStandard(session.PlayerData.Select(kvp => kvp.Value).ToList(), configuration.MaxParticipationStreak, configuration.SimultaneousPlays);
                    break;
                case RollingType.PluginWeightedRng:
                    session.PlayingPairs = rollManager.RollWeighted(session.PlayerData.Select(kvp => kvp.Value).ToList(), configuration.MaxParticipationStreak, configuration.SimultaneousPlays);
                    break;
                default:
                    chatGui.Print($"Rolling type not supported");
                    return;
            }
            AddParticipationRecords(session.PlayerData.Select(p => p.Value), session.PlayingPairs);
            IncreaseParticipationCounters();
            signManager.ApplyMarks(session.PlayingPairs);

            string optionalS = session.PlayingPairs.Count > 1 ? "S" : string.Empty;
            chatOutput.WriteChat($"-------------ROLLING NEW COUPLE{optionalS}--------------");
            chatOutput.WritePairs(session.PlayingPairs);
        }

        public void ReRoll(PlayerPair pair, bool rerollTheLoser)
        {
            log.Info($"[ACTION] Create timer: Reroll.");
            PlayerInfo? replaced = rerollTheLoser ? pair.Loser : pair.Winner;
            string rerrolledName = replaced?.FullName ?? "Nobody? This should not be possible.";
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

            MoveParticipations(replaced, replacement, !rerollTheLoser);

            chatOutput.WriteChat($"Rerroll! {replacement.FullName} replaces {rerrolledName}.");
        }

        public void CreateAndStartTimer(PlayerInfo target, string description, int minutes, int seconds)
        {
            log.Info($"[ACTION] Create timer: stopwatch based.");
            var duration = new TimeSpan(0, minutes, seconds);
            var timer = new TimerTimedAction(duration, target, description, CreateTimedActionCallback(target, description));
            timeKeeper.AddTimedAction(timer);
        }

        public void CreateAndStartTimer(PlayerInfo target, string description, int roundAmount)
        {
            log.Info($"[ACTION] Create timer: round based.");
            var timer = new RoundTimedAction(session.Round, roundAmount, target, description, CreateTimedActionCallback(target, description));
            timeKeeper.AddTimedAction(timer);
        }

        private OnTimedActionElapsed CreateTimedActionCallback(PlayerInfo target, string description)
        {            
            string descriptionSection = description.IsNullOrWhitespace() 
                ? string.Empty 
                : $"\"{description}\" ";
            return () =>
            {
                chatOutput.WriteChat($"{target.FullName.GetFirstName()}, you can stop {descriptionSection}now. <se.7>");
            };
        }

        public void ChatSoundWakeUp(PlayerInfo player)
        {
            log.Info($"[ACTION] Wake up through chat sound. Player: {player.FullName}.");
            chatOutput.ChatSoundWakeUp(player);
        }

        public void TellWakeUp(PlayerInfo player)
        {
            log.Info($"[ACTION] Wake up through tell. Player: {player.FullName}.");
            chatOutput.TellWakeUp(player);
        }

        private void AddParticipationRecords(IEnumerable<PlayerInfo> players, List<PlayerPair> pairs)
        {
            foreach (var player in session.PlayerData.Select(p => p.Value))
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
        }

        private void IncreaseParticipationCounters()
        {
            foreach (var player in session.PlayerData.Values)
            {
                player.ParticipationCounter.Total += 1;
            }

            foreach (var pair in session.PlayingPairs)
            {
                pair.Winner.ParticipationCounter.Wins += 1;
                if (pair.Loser != null)
                {
                    pair.Loser.ParticipationCounter.Losses += 1;
                }
            }
        }

        private void MoveParticipations(PlayerInfo? replaced, PlayerInfo replacement, bool isWinner)
        {
            if (replaced == null)
            {
                log.Warning($"Attempted to reroll a null player.");
                return;
            }
            replaced.ParticipationRecords.Last().Participation = RoundParticipation.NotParticipating;
            replacement.ParticipationRecords.Last().Participation = isWinner ? RoundParticipation.Winner : RoundParticipation.Loser;
            if (isWinner)
            {
                replaced.ParticipationCounter.Wins -= 1;
                replacement.ParticipationCounter.Wins += 1;
            }
            else
            {
                replaced.ParticipationCounter.Losses -= 1;
                replacement.ParticipationCounter.Losses += 1;
            }
        }
    }
}
