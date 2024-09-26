using Dalamud.Interface.Windowing;
using ECommons.GameHelpers;
using ImGuiNET;
using Model;
using System;
using System.Linq;
using System.Numerics;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Modules.Targeting;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;
using TruthOrDareHelper.Settings;
using TruthOrDareHelper.TestData;

namespace TruthOrDareHelper.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration configuration;
    private TruthOrDareSession session;
    private TargetManager targetManager;
    private ChatOutput chatOutput;
    private LogWrapper logWrapper;
    private ChatWrapper chatWrapper;

    private static readonly Vector4 Green = new Vector4(0, 1, 0, 0.6f);
    private static readonly Vector4 Red = new Vector4(1, 0, 0, 0.6f);
    private static readonly Vector4 Gray = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    private const string RoundSymbol = "♦";
    private const int RoundsToShowInHistory = 8;

    public MainWindow(Plugin plugin)
        : base("Truth or Dare helper", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
        configuration = plugin.Configuration;
        session = Plugin.Session;
        chatWrapper = Plugin.chatWrapper;
        logWrapper = Plugin.logWrapper;
        targetManager = Plugin.targetManager;
        chatOutput = Plugin.chatOutput;

        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(20), () => Plugin.Chat.PrintError("20s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(10), () => Plugin.Chat.PrintError("10s have passed")));
        //Plugin.timeKeeper.AddTimedAction(new TimerTimedAction(TimeSpan.FromSeconds(5), () => session.TryRemovePlayer("Player4")));
        //Plugin.timeKeeper.AddTimedAction(new RoundTimedAction(session.Round, 2, () => session.TryRemovePlayer("Player 4")));
    }

    public void Dispose()
    { }

    public override void Draw()
    {
        Plugin.timeKeeper.Tick(session.Round);
        DrawPlayerTable();
        if (ImGui.Button("Add target to the game"))
        {
            AddTargetPlayer();
        }

        ImGui.Separator();
        ImGui.TextUnformatted("This round");
        if (session.ArePlayersPaired())
        {
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
                DrawPairedPlayerCell(pair.Winner);

                ImGui.TableNextColumn();
                DrawPairedPlayerCell(pair.Loser);

                ImGui.TableNextColumn();
                ImGui.TextUnformatted("Truth"); // Truth/Dare/Any/Pending

                ImGui.TableNextColumn();

                bool referenceableDone = pair.Done;
                ImGui.Checkbox("## " + i, ref referenceableDone);
                pair.Done = referenceableDone;
            }

            ImGui.EndTable();
        }
    }

    private void DrawPairedPlayerCell(PlayerInfo? player)
    {
        bool playerNotChosenByRoll = player == null;
        string playerName = playerNotChosenByRoll ? "Not autodetected yet" : RemoveWorldFromName(player.FullName);
        ImGui.TextUnformatted(playerName);
        if (!playerNotChosenByRoll)
        {
            ImGui.SameLine();
            if (ImGui.Button(""))
            {
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
                ImGui.TextUnformatted(RemoveWorldFromName(player.FullName));
                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    if (targetManager.Target(player.FullName))
                    {
                        chatWrapper.Print($"Targeting {player.FullName}.");
                    }
                    else
                    {
                        chatWrapper.Print($"Could not target {player.FullName}.");
                    }
                }
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.GetIO().KeyShift)
                {
                    session.TryRemovePlayer(player.FullName);
                    targetManager.TryRemoveTargetReference(player.FullName);
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
        string? targetFullName = targetManager.AddReferenceToCurrentTarget();
        
        if (targetFullName == null)
        {
            chatWrapper.Print("Could not add target to the players. It's either nothing or not a player.");
            return false;
        }

        if (session.GetPlayer(targetFullName) != null)
        {
            chatWrapper.Print("Target player is already in the game.");
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
            ImGui.TextColored(color, "♦");
        }

        ImGui.EndGroup();
        Tooltip("Last 8 rounds. Green means being the asker, red the asked, gray not participating.");
    }

    private string RemoveWorldFromName(string name)
    {
        return name.Split("@").First();
    }

    private void Tooltip(string tooltip)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(tooltip);
        }
    }
}
