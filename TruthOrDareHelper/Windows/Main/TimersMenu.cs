using DalamudBasics.Extensions;
using ImGuiNET;
using Model;
using System.Collections.Generic;
using TruthOrDareHelper.Modules.TimeKeeping;
using TruthOrDareHelper.Modules.TimeKeeping.TimedActions;

namespace TruthOrDareHelper.Windows.Main
{
    public partial class MainWindow
    {
        private const string TimersPopupName = "Timers";
        private PlayerInfo? playerSelectedAsTimerTarget = null;
        private bool openTimersPopup = false;
        private int timedActionType = (int)TimedActionType.Rounds;
        private string timedActionDescription = "";
        private int timedActionRounds = 0;
        private int timedActionMinutes = 0;
        private int timedActionSeconds = 0;

        private void TriggerTimersPopupOpening(PlayerInfo target)
        {
            openTimersPopup = true;
            playerSelectedAsTimerTarget = target;
        }

        private void DrawTimersPopup()
        {
            if (openTimersPopup)
            {
                openTimersPopup = false;
                ImGui.OpenPopup(TimersPopupName);
            }
            if (ImGui.BeginPopup($"{TimersPopupName}", ImGuiWindowFlags.AlwaysAutoResize))
            {
                var player = playerSelectedAsTimerTarget;
                ImGui.TextColored(Yellow, player?.FullName ?? "Nobody");
                ImGui.SameLine();
                ImGui.TextUnformatted("Target");

                ImGui.PushItemWidth(ImGui.GetFontSize() * 15);
                ImGui.InputTextWithHint("Description", "What is this timer for (optional)", ref timedActionDescription, 240);
                ImGui.PopItemWidth();

                ImGui.RadioButton(" Count rounds", ref timedActionType, (int)TimedActionType.Rounds);
                ImGui.SameLine();
                ImGui.RadioButton(" Stopwatch", ref timedActionType, (int)TimedActionType.Time);

                if (timedActionType == (int)TimedActionType.Rounds)
                {
                    ImGui.PushItemWidth(ImGui.GetFontSize() * 6);
                    ImGui.InputInt("How many rounds?", ref timedActionRounds, 1, 1);
                    ImGui.PopItemWidth();
                }
                else if (timedActionType == (int)TimedActionType.Time)
                {
                    ImGui.PushItemWidth(ImGui.GetFontSize() * 6);
                    ImGui.InputInt("Minutes", ref timedActionMinutes, 1, 1);
                    ImGui.InputInt("Seconds", ref timedActionSeconds, 1, 1);
                    ImGui.PopItemWidth();
                }
                if (ImGui.Button($"Create timer##{player?.FullName ?? "nobody"}"))
                {
                    CreateTimer();
                    ImGui.CloseCurrentPopup();
                }

                DrawTimersTable(timeKeeper.Timers);

                ImGui.EndPopup();
            }
        }

        private void DrawTimersTable(ICollection<TimedAction> timedActions)
        {
            if (timedActions.Count == 0)
            {
                return;
            }

            const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;
            if (ImGui.BeginTable("##timers", 6, flags, new System.Numerics.Vector2(500, 20)))
            {
                ImGui.TableSetupColumn("Start time", ImGuiTableColumnFlags.WidthStretch, 0.25f);
                ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.WidthStretch, 0.8f);
                ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.5f);
                ImGui.TableSetupColumn("Duration", ImGuiTableColumnFlags.WidthStretch, 0.25f);
                ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthStretch, 0.25f);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 0.1f);
                ImGui.TableHeadersRow();

                foreach (var action in timedActions)
                {
                    string text = "";
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    text = action.StartTime.ToShortTimeString();
                    ImGui.TextUnformatted(text);
                    DrawTooltip(text);

                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted(action.Description);
                    DrawTooltip(action.Description);

                    ImGui.TableNextColumn();
                    text = action.Target.FullName.WithoutWorldName();
                    ImGui.TextUnformatted(text);
                    DrawTooltip(text);

                    ImGui.TableNextColumn();
                    if (action is TimerTimedAction tAction)
                    {
                        text = tAction.Duration.ToShortString();
                        ImGui.TextUnformatted(text);
                        DrawTooltip(text);

                        ImGui.TableNextColumn();
                        text = tAction.Remaining.ToShortString();
                        ImGui.TextUnformatted(text);
                        DrawTooltip(text);
                    }
                    else if (action is RoundTimedAction roundAction)
                    {
                        text = $"{roundAction.DurationInRounds} rnds";
                        ImGui.TextUnformatted(text);
                        DrawTooltip(text);

                        ImGui.TableNextColumn();
                        text = $"{roundAction.Remaining} rnds";
                        ImGui.TextUnformatted(text);
                        DrawTooltip(text);
                    }

                    ImGui.TableNextColumn();
                    DrawActionButton(() => runnerActions.RemoveTimer(action), "");
                    DrawTooltip("Cancel");
                }

                ImGui.EndTable();
            }
        }

        private void CreateTimer()
        {
            if (timedActionType == (int)TimedActionType.Rounds)
            {
                runnerActions.CreateAndStartTimer(playerSelectedAsTimerTarget, timedActionDescription, timedActionRounds);
                return;
            }

            runnerActions.CreateAndStartTimer(playerSelectedAsTimerTarget, timedActionDescription, timedActionMinutes, timedActionSeconds);
        }
    }
}
