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
        private PlayerInfo? playerSelectedAsTarget = null;
        private bool openTimersPopup = false;
        private int timedActionType = (int)TimedActionType.Rounds;
        private int timedActionRounds = 0;
        private int timedActionMinutes = 0;
        private int timedActionSeconds = 0;
               

        private void TriggerTimersPopupOpening(PlayerInfo target)
        {
            openTimersPopup = true;
            playerSelectedAsTarget = target;
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
                var player = playerSelectedAsTarget;
                ImGui.TextUnformatted("Target: ");
                ImGui.SameLine();
                ImGui.TextColored(Yellow, player?.FullName ?? "Nobody");

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
                    chatGui.Print(player.FullName);
                }

                ImGui.EndPopup();
            }
        }

        private void CreateTimer()
        {

        }

    }
}
