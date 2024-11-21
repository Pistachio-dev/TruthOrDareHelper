using DalamudBasics.Extensions;
using DalamudBasics.GUI.Forms;
using ImGuiNET;
using Model;
using System.Numerics;
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
            if (ImGui.BeginPopup($"{TimersPopupName}"))
            {
                var player = playerSelectedAsTimerTarget;
                ImGui.TextUnformatted("Target: ");
                ImGui.SameLine();
                ImGui.TextColored(Yellow, player?.FullName ?? "Nobody");
                ImGui.InputTextWithHint("Description", "What is this timing, optional", ref timedActionDescription, 240);
                ImGui.TextUnformatted("Timer type: ");
                ImGui.SameLine();
                ImGui.RadioButton("Rounds", ref timedActionType, (int)TimedActionType.Rounds);
                ImGui.SameLine();
                ImGui.RadioButton("Time", ref timedActionType, (int)TimedActionType.Time);
                if (timedActionType == (int)TimedActionType.Rounds)
                {
                    ImGui.InputInt("How many rounds?", ref timedActionRounds, 1, 1);
                }
                else if (timedActionType == (int)TimedActionType.Time)
                {
                    ImGui.InputInt("Minutes", ref timedActionMinutes, 1, 1);
                    ImGui.InputInt("Seconds", ref timedActionSeconds, 1, 1);
                }
                if (ImGui.Button($"Create timer##{player?.FullName ?? "nobody"}"))
                {
                    CreateTimer();
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
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
