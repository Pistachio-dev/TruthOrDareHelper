using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace TruthOrDareHelper.Windows.Main
{
    public partial class MainWindow
    {
        private const string PromptsPopupName = "Prompts";
        private bool openPrompsPopup = false;

        private void DrawPromptsPopup()
        {            
            if (openPrompsPopup)
            {
                ImGui.OpenPopup(PromptsPopupName);
                openPrompsPopup = false;
            }
            if (ImGui.BeginPopup(PromptsPopupName))
            {
                ImGui.TextUnformatted(prompter.GetStatsString());
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 72 / 255f, 0, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 102 / 255f, 0, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 72 / 255f, 0, 1));
                try
                {
                    DrawActionButton(() => runnerActions.ReloadPrompts(), "Reload from files");
                }
                finally
                {
                    ImGui.PopStyleColor(3);
                }
                
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(220/255f, 40/255f, 40 / 255f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(220 / 255f, 40 / 255f, 40 / 255f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(220 / 255f, 40 / 255f, 40 / 255f, 1));
                ImGui.SameLine();
                try
                {
                    DrawActionButton(() => runnerActions.OpenPromptsFolder(), "Open prompts folder");
                }
                finally
                {
                    ImGui.PopStyleColor(3);
                }
                
                ImGui.TextColored(Yellow, "Every line on each file is read as a prompt, feel free to add your own or remove some");

                ImGui.EndPopup();
            }
        }
    }
}
