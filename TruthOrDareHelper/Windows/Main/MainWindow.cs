using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System;
using System.Linq;
using System.Numerics;
using TruthOrDareHelper.GameActions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Prompting.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Windows.Main;

public partial class MainWindow : PluginWindowBase, IDisposable
{
    private bool compactMode;

    private static readonly Vector4 Green = new Vector4(0, 1, 0, 0.6f);
    private static readonly Vector4 Red = new Vector4(1, 0, 0, 0.6f);
    private static readonly Vector4 Gray = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    private static readonly Vector4 LightGreen = new Vector4(162 / 255f, 1, 153 / 255f, 1);
    private static readonly Vector4 Pink = new Vector4(1, 160f / 255, 160 / 255f, 1);
    private static readonly Vector4 Yellow = new Vector4(1, 1, 153 / 255f, 1);
    private Plugin plugin;
    private ITruthOrDareSession session;
    private IRollManager rollManager;
    private Configuration configuration;
    private IToDChatOutput toDChatOutput;
    private IClientChatGui chatGui;
    private ITargetingService targetManager;
    private const string RoundSymbol = "♦";
    private const int RoundsToShowInHistory = 8;
    private IRunnerActions runnerActions;
    private IPrompter prompter;

    public MainWindow(Plugin plugin, ILogService logService, IServiceProvider serviceProvider)
        : base(logService, "Truth or Dare helper", ImGuiWindowFlags.AlwaysAutoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.SizeCondition = ImGuiCond.Always;
        this.plugin = plugin;
        session = serviceProvider.GetRequiredService<ITruthOrDareSession>();
        rollManager = serviceProvider.GetRequiredService<IRollManager>();
        configuration = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>().GetConfiguration();
        toDChatOutput = serviceProvider.GetRequiredService<IToDChatOutput>();
        chatGui = serviceProvider.GetRequiredService<IClientChatGui>();
        targetManager = serviceProvider.GetRequiredService<ITargetingService>();
        runnerActions = serviceProvider.GetRequiredService<IRunnerActions>();
        prompter = serviceProvider.GetRequiredService<IPrompter>();

        InitializeFormFactory();
    }

    private void GoLalaMode()
    {
        compactMode = true;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(100, 100),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    private void GoBigMode()
    {
        compactMode = false;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    private void ToggleSize()
    {
        if (compactMode)
        {
            compactMode = false;
            GoBigMode();
        }
        else
        {
            compactMode = true;
            GoLalaMode();
        }
    }

    public void Dispose()
    { }

    protected override void SafeDraw()
    {
        DrawPlayerAcceptedTopicsPopup();
        DrawTimersPopup();
        DrawPromptsPopup();
        DrawPlayerTable();
        DrawHelpPopup();

        ImGui.TextColored(Yellow, $"Round {session.Round}");        

        ImGui.SameLine();
        ImGui.PushID("AddTarget##1");
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0.5f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0.5f, 0.7f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0.5f, 0.7f));
        bool targetingAPlayer = targetManager.IsTargetingAPlayer();
        DrawWithinDisableBlock(targetingAPlayer, () =>
        {
            string addPlayerText = compactMode ? "" : " Add target to game";
            DrawActionButton(() => AddTargetPlayer(), addPlayerText);
        });
        if (!targetingAPlayer)
        {
            DrawTooltip("Add your target to the players. This only works if you target a player.");
        }
        else
        {
            DrawTooltip("Add your target to the players.");
        }

        ImGui.PopStyleColor(3);
        ImGui.PopID();

        if (session.PlayerData.Count >= 2)
        {
            ImGui.SameLine();
            ImGui.PushID("RollButton");
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0.5f, 0, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0.5f, 0, 0.7f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0.5f, 0, 0.7f));

            string rollText = compactMode ? "" : " Roll a new round ";
            DrawActionButton(() => runnerActions.Roll(), rollText);
            DrawTooltip("Start a new round and form new player pairs. Rolling type can be changed in Configuration.");
            ImGui.PopStyleColor(3);
            ImGui.PopID();
        }

        ImGui.SameLine();
        string configText = compactMode ? "" : " Configuration";
        DrawActionButton(() => plugin.ToggleConfigUI(), configText);
        DrawTooltip("Configuration");

        ImGui.SameLine();
        string promptButtonText = compactMode ? "¶" : "¶ Prompts";
        DrawActionButton(() => openPrompsPopup = true, promptButtonText);
        DrawTooltip("Prompts are ideas players can get if they can't come with one themselves.");

        ImGui.SameLine();
        DrawActionButton(() => ToggleSize(), compactMode ? " Grow" : " Lala mode");

        ImGui.SameLine();
        DrawActionButton(() => openHelpPopup = true, "");
        DrawTooltip("Help");

        ImGui.Separator();
        ImGui.TextUnformatted("This round");

        if (session.ArePlayersPaired())
        {
            ImGui.SameLine();
            if (ImGui.Button($" Write pairs to chat"))
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
        if (ImGui.BeginTable("##PairedPlayerTable", 5, flags))
        {
            ImGui.TableSetupColumn("Asker", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Target", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Choice", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("Done", ImGuiTableColumnFlags.WidthStretch, 0.1f);
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthStretch, 0.1f);

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

                ImGui.TableNextColumn();
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 0));
                try
                {
                    DrawWithinDisableBlock(pair.Loser != null, () =>
                    {
                        if (ImGui.Button($"##{pair.Winner.FullName}"))
                        {
                            TriggerTimersPopupOpening(pair.Loser!);
                        }
                        DrawTooltip("Start a timer");
                    });

                    ImGui.SameLine();
                    DrawActionButton(() => runnerActions.WritePrompt(pair.Loser ?? pair.Winner, pair.ChallengeType), $"##{pair.Winner.FullName}");
                }
                finally
                {
                    ImGui.PopStyleVar();
                }

                DrawTooltip("Give the player a random suggestion for their truth or dare if they're drawing a blank.");
            }

            ImGui.EndTable();
        }
    }

    private void DrawPairedPlayerCell(PlayerPair pair, bool isLoser, int row)
    {
        var player = isLoser ? pair.Loser : pair.Winner;
        var playerNotChosenByRoll = player == null;
        var playerName = playerNotChosenByRoll ? "Not autodetected yet" : player.FullName.WithoutWorldName();
        if (compactMode)
        {
            playerName = playerName.GetFirstName();
        }

        ImGui.TextUnformatted(playerName);
        if (!playerNotChosenByRoll)
        {
            ImGui.SameLine();
            string rerollButtonText = compactMode ? "" : "";
            DrawActionButton(() => runnerActions.ReRoll(pair, isLoser), $"{rerollButtonText}##{row}{isLoser}");
            DrawTooltip("Reroll player, if this one is afk or passes.");
        }
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
        int amountOfColumns = compactMode ? 2 : 7;
        if (ImGui.BeginTable("##PlayerTable", amountOfColumns, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            if (!compactMode)
            {
                ImGui.TableSetupColumn("NSFW?", ImGuiTableColumnFlags.WidthFixed, 0.2f);
                ImGui.TableSetupColumn("Wins", ImGuiTableColumnFlags.WidthStretch, 0.1f);
                ImGui.TableSetupColumn("Losses", ImGuiTableColumnFlags.WidthStretch, 0.1f);
                ImGui.TableSetupColumn("History", ImGuiTableColumnFlags.WidthStretch, 0.5f);
                ImGui.TableSetupColumn("Playing", ImGuiTableColumnFlags.WidthStretch, 0.2f);
                ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            }

            ImGui.TableHeadersRow();

            foreach (var player in session.PlayerData.Values)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                string playerName = compactMode ? player.FullName.GetFirstName() : player.FullName.WithoutWorldName();
                ImGui.TextUnformatted(playerName);
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
                    toDChatOutput.WriteChat($"{player.FullName} leaves the game.");
                }
                DrawTooltip("Click to target the player, shift + right click to remove them.");
                if (!compactMode)
                {
                    ImGui.TableNextColumn();
                    DrawAcceptedTopicsCell(player);

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

                ImGui.TableNextColumn();
                Vector2 buttonSize = new Vector2(18, 18);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 0));
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1, 0));
                if (ImGui.Button($"##{player.FullName}"))
                {
                    TriggerTimersPopupOpening(player);
                }
                DrawTooltip("Start a timer");
                ImGui.SameLine();
                DrawAcceptedTopicsPopupButton(player);
                ImGui.SameLine();

                ImGui.PushID($"SmallWakeUpButton#{player.FullName}");
                ImGui.SameLine();
                if (ImGui.Button($"!", buttonSize))
                {
                    runnerActions.ChatSoundWakeUp(player);
                }
                DrawTooltip("Play wake up sound");

                ImGui.PushID($"BigWakeUpButton#{player.FullName}");
                ImGui.SameLine();
                if (ImGui.Button("!!", buttonSize))
                {
                    runnerActions.TellWakeUp(player);
                }
                DrawTooltip("Send a wake up /tell");
                ImGui.PopStyleVar(2);
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

        session.AddNewPlayer(targetFullName, configuration.DefaultTruthAcceptance, configuration.DefaultDareAcceptance);
        toDChatOutput.WriteChat($"{targetFullName} joins the game.");

        return true;
    }

    private void DrawRoundHistory(PlayerInfo player)
    {
        var roundsToSkip = Math.Max(player.ParticipationRecords.Count - RoundsToShowInHistory, 0);
        ImGui.BeginGroup();
        bool firstIteration = true;
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 0));
        foreach (var roundRecord in player.ParticipationRecords.Skip(roundsToSkip))
        {
            var color = roundRecord.Participation switch
            {
                RoundParticipation.Winner => Green,
                RoundParticipation.Loser => Red,
                _ => Gray,
            };
            if (!firstIteration)
            {
                ImGui.SameLine();
            }

            ImGui.TextColored(color, RoundSymbol);
            firstIteration = false;
        }
        ImGui.PopStyleVar();
        ImGui.EndGroup();
        DrawTooltip("Last 8 rounds. Green means being the asker, red the asked, gray not participating.");
    }
}
