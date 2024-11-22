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
using TruthOrDareHelper.Modules.Prompting.Interface;
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
        private readonly IPrompter prompter;
        private readonly Configuration configuration;

        public RunnerActions(ITruthOrDareSession session, IConfigurationService<Configuration> configService, IToDChatOutput chatOutput, IRollManager rollManager, ISignManager signManager,
            ITargetingService targetingManager, ILogService log, IClientChatGui chatGui, ITimeKeeper timeKeeper, IPrompter prompter)
        {
            this.session = session;
            this.chatOutput = chatOutput;
            this.rollManager = rollManager;
            this.signManager = signManager;
            this.targetingManager = targetingManager;
            this.log = log;
            this.chatGui = chatGui;
            this.timeKeeper = timeKeeper;
            this.prompter = prompter;
            this.configuration = configService.GetConfiguration();
        }

        public void CompletePair(PlayerPair pair)
        {
            pair.Done = true;
            if (configuration.AutoRollOnAllComplete && !session.PlayingPairs.Any(p => !p.Done))
            {
                chatOutput.WriteChat($"Everyone is done! Starting new round in 3", null, 1000);
                chatOutput.WriteChat($"2", null, 1000);
                chatOutput.WriteChat($"1", null, 1000);
                chatOutput.WriteChat($"Go!", null, 1000);                
                Roll();
            }
        }

        public void PrintChatCommands()
        {
            log.Info($"[ACTION] Print chat commands.");
            string line1 = "\"truth\", \"dare\", \"dealer's choice\" (or \"t\", \"d\", \"dch\") to choose truth or dare.";
            string line2 = "\"coinflip\" to have the plogon decide for you";
            string line3 = $"\"{configuration.ConfirmationKeyword}\" as asker to confirm whatever you asked was done";
            string line4 = $"\"hint\" to get a random suggestion. You can also narrow it like \"hint nsfw dare\" or \"hint sfw truth\"";
            chatOutput.WriteChat(line1);
            chatOutput.WriteChat(line2);
            chatOutput.WriteChat(line3);
            chatOutput.WriteChat(line4);
        }

        public void RemovePlayer(PlayerInfo player)
        {            
            signManager.UnmarkPlayer(player);
            
            PlayerPair? pairIncludingPlayer = session.PlayingPairs.FirstOrDefault(pp => pp.Winner == player || pp.Loser == player);
            if (pairIncludingPlayer != null)
            {
                signManager.UnmarkPlayer(pairIncludingPlayer.Winner);
                if (pairIncludingPlayer.Loser != null)
                {
                    signManager.UnmarkPlayer(pairIncludingPlayer.Loser);
                }
                session.PlayingPairs.Remove(pairIncludingPlayer);
            }

            targetingManager.RemovePlayerReference(player.FullName);
            session.TryRemovePlayer(player.FullName);
            timeKeeper.RemoveTimersForPlayer(player);
            chatOutput.WriteChat($"{player.FullName} leaves the game.");
        }

        public void EndGame()
        {           
            foreach (var player in session.PlayerData.Values)
            {
                signManager.UnmarkPlayer(player);
                targetingManager.RemovePlayerReference(player.FullName);
            }

            session.Reset();
            chatGui.Print("Game finished and players removed");
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
            chatGui.Print("Timer created and started");
        }

        public void CreateAndStartTimer(PlayerInfo target, string description, int roundAmount)
        {
            log.Info($"[ACTION] Create timer: round based.");
            var timer = new RoundTimedAction(session.Round, roundAmount, target, description, CreateTimedActionCallback(target, description));
            timeKeeper.AddTimedAction(timer);
            chatGui.Print("Timer created and started");
        }

        public void RemoveTimer(TimedAction timer)
        {
            log.Info($"[ACTION] Remove timer with id {timer.Id} and description {timer.Description}.");
            timeKeeper.RemoveTimedAction(timer);
            chatGui.Print($"Timer for \"{timer.Description}\" cancelled");
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

        public void ReloadPrompts()
        {
            log.Info($"[ACTION] Reload prompts.");
            prompter.LoadPromptsToMemory();
            var statsDescription = prompter.GetStatsString();
            chatGui.Print(statsDescription);
        }

        public void OpenPromptsFolder()
        {
            log.Info($"[ACTION] Open prompt folder.");
            prompter.OpenFolder();
        }

        public void WritePrompt(PlayerInfo loser, ChallengeType challengeType)
        {
            log.Info($"[ACTION] Requesting prompt for player {loser.FullName}.");
            var prompt = "I've got nothing";
            if (challengeType == ChallengeType.Truth)
            {
                prompt = prompter.GetPrompt(loser.AcceptsSfwTruth, loser.AcceptsNsfwTruth, false, false);
            }
            else if (challengeType == ChallengeType.Dare)
            {
                prompt = prompter.GetPrompt(false, false, loser.AcceptsSfwDare, loser.AcceptsNsfwDare);
            }
            else
            {
                prompt = prompter.GetPrompt(loser.AcceptsSfwTruth, loser.AcceptsNsfwTruth, loser.AcceptsSfwDare, loser.AcceptsNsfwDare);
            }

            chatOutput.WriteChat($"{prompt}");
        }

        public void ToggleAFK(PlayerInfo player)
        {
            log.Info($"[ACTION] Toggle AFK. Player: {player.FullName}.");
            player.AFK = !player.AFK;
            if (player.AFK)
            {
                chatOutput.WriteChat($"{player.FullName.WithoutWorldName()} is AFK.");
            }
            else
            {
                chatOutput.WriteChat($"{player.FullName.WithoutWorldName()} is no longer AFK.");
            }
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
