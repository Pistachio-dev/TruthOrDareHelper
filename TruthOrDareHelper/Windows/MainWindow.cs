using Dalamud.Game.Text;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Windows;

public class MainWindow : PluginWindowBase, IDisposable
{
    private static readonly Vector4 Green = new Vector4(0, 1, 0, 0.6f);
    private static readonly Vector4 Red = new Vector4(1, 0, 0, 0.6f);
    private static readonly Vector4 Gray = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    private Plugin plugin;
    private ITruthOrDareSession session;
    private IRollManager rollManager;
    private IChatOutput chatOutput;
    private Configuration configuration;
    private IToDChatOutput toDChatOutput;
    private IClientChatGui chatGui;
    private ITargetingService targetManager;
    private const string RoundSymbol = "♦";
    private const int RoundsToShowInHistory = 8;

    public MainWindow(Plugin plugin, ILogService logService, IServiceProvider serviceProvider)
        : base(logService, "Truth or Dare helper")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        session = serviceProvider.GetRequiredService<ITruthOrDareSession>();
        rollManager = serviceProvider.GetRequiredService<IRollManager>();
        chatOutput = serviceProvider.GetRequiredService<IChatOutput>();
        configuration = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>().GetConfiguration();
        toDChatOutput = serviceProvider.GetRequiredService<IToDChatOutput>();
        chatGui = serviceProvider.GetRequiredService<IClientChatGui>();
        targetManager = serviceProvider.GetRequiredService<ITargetingService>();

        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(20), () => Plugin.Chat.PrintError("20s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(10), () => Plugin.Chat.PrintError("10s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(5), () => session.TryRemovePlayer("Player4")));
        //Plugin.timeKeeper.AddTimedAction(new RoundTimedAction(session.Round, 2, () => session.TryRemovePlayer("Player 4")));
    }

    public void Dispose()
    { }

    private void Roll()
    {
        session.Round++;
        // TODO: Before next roll, make sure to add the "truth wins, dare wins, etc" stats if available
        List<PlayerPair> pairs = rollManager.RollStandard(session.PlayerInfo.Select(kvp => kvp.Value).ToList(), configuration.MaxParticipationStreak, configuration.SimultaneousPlays);
        foreach (var player in session.PlayerInfo.Select(p => p.Value))
        {
            if (pairs.FirstOrDefault(p => p.Winner == player) != null)
            {
                player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.Winner));
                AddConfirmationDetection(player.FullName.WithoutWorldName());
            }
            else if (pairs.FirstOrDefault(p => p.Loser == player) != null)
            {
                player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.Loser));
                AddToDChoiceDetection(player.FullName.WithoutWorldName());
            }
            else
            {
                player.ParticipationRecords.Add(new RoundParticipationRecord(session.Round, RoundParticipation.NotParticipating));
            }
        }

        session.PlayingPairs = pairs;
        chatOutput.WriteChat($"-------------ROLLING--------------");
        //chatOutput.WritePairs(pairs);
    }

    private void AddToDChoiceDetection(string playerName)
    {
        //ConditionalDelegatePayload payload = new ConditionalDelegatePayload(null, null, playerName, DetectTruthOrDare);
        //chatListener.AddConditionalDelegate(payload);
        //log.Info($"Added ToD choice chat listener for {playerName}");
    }

    private void AddConfirmationDetection(string playerName)
    {
        //ConditionalDelegatePayload payload = new ConditionalDelegatePayload(null, null, playerName, DetectConfirmation);
        //chatListener.AddConditionalDelegate(payload);
        //log.Info($"Added ToD confirmation chat listener for {playerName}");
    }

    private bool DetectTruthOrDare(XivChatEntry chatChannel, DateTime timeStamp, string sender, string message)
    {
        PlayerPair? pair = session.PlayingPairs.FirstOrDefault(p => (p.Loser?.FullName.Contains(sender) ?? false));
        if (pair == null)
        {
            // This would happen in case of rerrolls.
            return true;
        }

        if (message.Equals("t", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("truth", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("truht", StringComparison.InvariantCultureIgnoreCase))
        {
            pair.ChallengeType = ChallengeType.Truth;
            logService.Info($"{pair.Loser.FullName} chose TRUTH");
            return true;
        }
        if (message.Equals("d", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("dare", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("daer", StringComparison.InvariantCultureIgnoreCase))
        {
            pair.ChallengeType = ChallengeType.Dare;
            logService.Info($"{pair.Loser.FullName} chose DARE");

            return true;
        }
        if (message.Contains("choice", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("decide", StringComparison.InvariantCultureIgnoreCase)
            || message.Contains("dealer", StringComparison.InvariantCultureIgnoreCase))
        {
            pair.ChallengeType = ChallengeType.DealersChoice;
            logService.Info($"{pair.Loser.FullName} chose Dealer's choice");

            return true;
        }

        return false;
    }

    private bool DetectConfirmation(XivChatType chatChannel, DateTime timeStamp, string sender, string message)
    {
        PlayerPair? pair = session.PlayingPairs.FirstOrDefault(p => (p.Winner?.FullName.Contains(sender) ?? false));
        if (pair == null)
        {
            // This would happen in case of rerrolls.
            return true;
        }

        if (message.Contains(configuration.ConfirmationKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            pair.Done = true;
            logService.Info($"{pair.Winner.FullName} confirms the answer as valid!");

            if (session.PlayingPairs.FirstOrDefault(pair => !pair.Done) == null)
            {
                Roll();
            }

            return true;
        }

        return false;
    }

    private void ReRoll(PlayerPair pair, bool rerollTheLoser)
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

        chatOutput.WriteChat($"{replacement.FullName} replaces {rerrolledName}.");

        if (pair.ChallengeType != ChallengeType.None && rerollTheLoser)
        {
            AddToDChoiceDetection(rerrolledName.WithoutWorldName());
        }
    }

    protected override void SafeDraw()
    {
        DrawPlayerTable();

        ImGui.TextUnformatted($"Round {session.Round}");
        if (session.PlayerInfo.Count >= 2)
        {
            ImGui.SameLine();
            ImGui.PushID("RollButton");
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0.5f, 0, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0.5f, 0, 0.7f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0.5f, 0, 0.7f));

            if (ImGui.Button("New round, rrrrrolll the dice!"))
            {
                Roll();
            }
            ImGui.PopStyleColor(3);
            ImGui.PopID();
        }

        ImGui.SameLine();
        ImGui.PushID("AddTarget##1");
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0.5f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0.5f, 0.7f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0.5f, 0.7f));
        if (ImGui.Button("Add target player to the game"))
        {
            AddTargetPlayer();
        }
        ImGui.PopStyleColor(3);
        ImGui.PopID();

        ImGui.SameLine();
        if (ImGui.Button("Configuration"))
        {
            plugin.ToggleConfigUI();
        }

        ImGui.Separator();
        ImGui.TextUnformatted("This round");

        if (session.ArePlayersPaired())
        {
            ImGui.SameLine();
            if (ImGui.Button($"Print player pairs to chat"))
            {
                toDChatOutput.WritePairs(session.PlayingPairs);
            }
            DrawPlayingPairsTable();
        }
        else
        {
            ImGui.TextUnformatted("Player pairs not yet formed.");
        }
    }

    private void DrawPlayingPairsTable()
    {
        bool[] check = session.PlayingPairs.Select(p => p.Done).ToArray();
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        if (ImGui.BeginTable("##PairedPlayerTable", 4, flags))
        {
            ImGui.TableSetupColumn("Asker", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Target", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Choice", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("Done", ImGuiTableColumnFlags.WidthStretch, 0.1f); // Use a checkbox here

            ImGui.TableHeadersRow();

            for (int i = 0; i < session.PlayingPairs.Count; i++)
            {
                var pair = session.PlayingPairs[i];

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                DrawPairedPlayerCell(pair, isLoser: false, row: i);

                ImGui.TableNextColumn();
                DrawPairedPlayerCell(pair, isLoser: true, row: i);

                ImGui.TableNextColumn();
                string challengeText = pair.ChallengeType switch
                {
                    ChallengeType.None => "?",
                    ChallengeType.DealersChoice => "Any",
                    ChallengeType.Dare => "Dare",
                    ChallengeType.Truth => "Truth"
                };

                ImGui.TextUnformatted(challengeText);

                ImGui.TableNextColumn();

                bool referenceableDone = pair.Done;
                ImGui.Checkbox("## " + i, ref referenceableDone);
                pair.Done = referenceableDone;
            }

            ImGui.EndTable();
        }
    }

    private void DrawPairedPlayerCell(PlayerPair pair, bool isLoser, int row)
    {
        PlayerInfo? player = isLoser ? pair.Loser : pair.Winner;
        bool playerNotChosenByRoll = player == null;
        string playerName = playerNotChosenByRoll ? "Not autodetected yet" : player.FullName.WithoutWorldName();
        ImGui.TextUnformatted(playerName);
        if (!playerNotChosenByRoll)
        {
            ImGui.SameLine();
            if (ImGui.Button($"##{row}{isLoser}"))
            {
                ReRoll(pair, isLoser);
            }

            Tooltip("Reroll player, if this one is afk or passes.");
        }
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        if (ImGui.BeginTable("##PlayerTable", 5, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("Wins", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("Losses", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("History", ImGuiTableColumnFlags.WidthStretch, 0.5f);
            ImGui.TableSetupColumn("Playing", ImGuiTableColumnFlags.WidthStretch, 0.2f);

            ImGui.TableHeadersRow();

            foreach (var player in session.PlayerInfo.Values)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(player.FullName.WithoutWorldName());
                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    if (targetManager.TargetPlayer(player.FullName))
                    {
                        chatGui.Print($"Targeting {player.FullName}.");
                    }
                    else
                    {
                        chatGui.Print($"Could not target {player.FullName}.");
                    }
                }
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.GetIO().KeyShift)
                {
                    session.TryRemovePlayer(player.FullName);
                    targetManager.RemovePlayerReference(player.FullName);
                    chatOutput.WriteChat($"{player.FullName} leaves the game.");
                }
                Tooltip("Click to target the player, shift + right click to remove them.");

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(player.Wins.ToString());

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(player.Losses.ToString());

                ImGui.TableNextColumn();
                DrawRoundHistory(player);

                ImGui.TableNextColumn();
                if (session.IsPlayerPlaying(player))
                {
                    ImGui.TextColored(Green, "✓");
                }
                else
                {
                    ImGui.TextColored(Gray, "x");
                }
            }

            ImGui.EndTable();
        }
    }

    private bool AddTargetPlayer()
    {
        if (!targetManager.TrySaveTargetPlayerReference(out var targetReference) || targetReference == null)
        {
            chatGui.Print("Could not add target to the players. It's either nothing or not a player.");
            return false;
        }

        string targetFullName = targetReference.GetFullName();
        if (session.GetPlayer(targetFullName) != null)
        {
            chatGui.Print("Target player is already in the game.");
            return true;
        }

        session.AddNewPlayer(targetFullName);
        chatOutput.WriteChat($"{targetFullName} joins the game.");

        return true;
    }

    private void DrawRoundHistory(PlayerInfo player)
    {
        int roundsToSkip = Math.Max(player.ParticipationRecords.Count - RoundsToShowInHistory, 0);
        ImGui.BeginGroup();
        foreach (RoundParticipationRecord roundRecord in player.ParticipationRecords.Skip(roundsToSkip))
        {
            Vector4 color = roundRecord.Participation switch
            {
                RoundParticipation.Winner => Green,
                RoundParticipation.Loser => Red,
                _ => Gray,
            };
            ImGui.SameLine();
            ImGui.TextColored(color, RoundSymbol);
        }

        ImGui.EndGroup();
        Tooltip("Last 8 rounds. Green means being the asker, red the asked, gray not participating.");
    }

    private void Tooltip(string tooltip)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(tooltip);
        }
    }
}
