using ImGuiNET;
using Model;
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
