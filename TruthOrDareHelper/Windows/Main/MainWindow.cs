using Dalamud.Game.Text;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using ECommons.GameHelpers;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TruthOrDareHelper.GameActions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Windows.Main;

public partial class MainWindow : PluginWindowBase, IDisposable
{
    private static readonly Vector4 Green = new Vector4(0, 1, 0, 0.6f);
    private static readonly Vector4 Red = new Vector4(1, 0, 0, 0.6f);
    private static readonly Vector4 Gray = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    private static readonly Vector4 LightGreen = new Vector4(162 / 255f, 1, 153 / 255f, 1);
    private static readonly Vector4 Pink = new Vector4(1, 160f/255, 160 / 255f, 1);
    private static readonly Vector4 Yellow = new Vector4(1, 1, 153 / 255f, 1);
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
    private IRunnerActions runnerActions;

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
        runnerActions = serviceProvider.GetRequiredService<IRunnerActions>();

        InitializeFormFactory();

        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(20), () => Plugin.Chat.PrintError("20s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(10), () => Plugin.Chat.PrintError("10s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(5), () => session.TryRemovePlayer("Player4")));
        //Plugin.timeKeeper.AddTimedAction(new RoundTimedAction(session.Round, 2, () => session.TryRemovePlayer("Player 4")));
    }

    public void Dispose()
    { }

    protected override void SafeDraw()
    {
        DrawPlayerAcceptedTopicsPopup();
        if (ImGui.Button("Test chat"))
        {
            chatOutput.WriteChat(Guid.NewGuid().ToString(), XivChatType.Say);
        };
        DrawPlayerTable();

        ImGui.TextUnformatted($"Round {session.Round}");
        if (session.PlayerData.Count >= 2)
        {
            ImGui.SameLine();
            ImGui.PushID("RollButton");
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0.5f, 0, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0.5f, 0, 0.7f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0.5f, 0, 0.7f));

            DrawActionButton(() => runnerActions.Roll(), "New round, rrrrrolll the dice!");
            ImGui.PopStyleColor(3);
            ImGui.PopID();
        }

        ImGui.SameLine();
        ImGui.PushID("AddTarget##1");
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0.5f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0.5f, 0.7f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0.5f, 0.7f));
        DrawActionButton(() => AddTargetPlayer(), "Add target player to the game");
        ImGui.PopStyleColor(3);
        ImGui.PopID();

        ImGui.SameLine();
        DrawActionButton(() => plugin.ToggleConfigUI(), "Configuration");

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
        var check = session.PlayingPairs.Select(p => p.Done).ToArray();
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        if (ImGui.BeginTable("##PairedPlayerTable", 4, flags))
        {
            ImGui.TableSetupColumn("Asker", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Target", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Choice", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("Done", ImGuiTableColumnFlags.WidthStretch, 0.1f); // Use a checkbox here

            ImGui.TableHeadersRow();

            for (var i = 0; i < session.PlayingPairs.Count; i++)
            {
                var pair = session.PlayingPairs[i];

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                DrawPairedPlayerCell(pair, isLoser: false, row: i);

                ImGui.TableNextColumn();
                DrawPairedPlayerCell(pair, isLoser: true, row: i);

                ImGui.TableNextColumn();
                var challengeText = pair.ChallengeType switch
                {
                    ChallengeType.None => "?",
                    ChallengeType.DealersChoice => "Any",
                    ChallengeType.Dare => "Dare",
                    ChallengeType.Truth => "Truth"
                };

                ImGui.TextUnformatted(challengeText);

                ImGui.TableNextColumn();

                var referenceableDone = pair.Done;
                ImGui.Checkbox("## " + i, ref referenceableDone);
                pair.Done = referenceableDone;
            }

            ImGui.EndTable();
        }
    }

    private void DrawPairedPlayerCell(PlayerPair pair, bool isLoser, int row)
    {
        var player = isLoser ? pair.Loser : pair.Winner;
        var playerNotChosenByRoll = player == null;
        var playerName = playerNotChosenByRoll ? "Not autodetected yet" : player.FullName.WithoutWorldName();
        ImGui.TextUnformatted(playerName);
        if (!playerNotChosenByRoll)
        {
            ImGui.SameLine();
            DrawActionButton(() => runnerActions.ReRoll(pair, isLoser), $"##{row}{isLoser}");
            DrawTooltip("Reroll player, if this one is afk or passes.");
        }
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        if (ImGui.BeginTable("##PlayerTable", 6, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("NSFW?", ImGuiTableColumnFlags.WidthFixed, 0.2f);
            ImGui.TableSetupColumn("Wins", ImGuiTableColumnFlags.WidthStretch, 0.1f);
            ImGui.TableSetupColumn("Losses", ImGuiTableColumnFlags.WidthStretch, 0.1f);
            ImGui.TableSetupColumn("History", ImGuiTableColumnFlags.WidthStretch, 0.5f);
            ImGui.TableSetupColumn("Playing", ImGuiTableColumnFlags.WidthStretch, 0.2f);

            ImGui.TableHeadersRow();

            foreach (var player in session.PlayerData.Values)
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
                DrawTooltip("Click to target the player, shift + right click to remove them.");

                ImGui.TableNextColumn();
                DrawActionButton(() => {
                    playerSelectedForTopicsAcceptedMenu = player;
                    openAcceptedTopicsDialogue = true;
                    }, $"Check##{player.FullName}");                

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

        var targetFullName = targetReference.GetFullName();
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
        var roundsToSkip = Math.Max(player.ParticipationRecords.Count - RoundsToShowInHistory, 0);
        ImGui.BeginGroup();
        foreach (var roundRecord in player.ParticipationRecords.Skip(roundsToSkip))
        {
            var color = roundRecord.Participation switch
            {
                RoundParticipation.Winner => Green,
                RoundParticipation.Loser => Red,
                _ => Gray,
            };
            ImGui.SameLine();
            ImGui.TextColored(color, RoundSymbol);
        }

        ImGui.EndGroup();
        DrawTooltip("Last 8 rounds. Green means being the asker, red the asked, gray not participating.");
    }

}