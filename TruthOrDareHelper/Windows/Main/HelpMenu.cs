using ImGuiNET;

namespace TruthOrDareHelper.Windows.Main
{
    public partial class MainWindow
    {
        private const string HelpMenuName = "Help";
        private bool openHelpPopup = false;

        private void DrawHelpPopup()
        {
            if (openHelpPopup)
            {
                ImGui.OpenPopup(HelpMenuName);
                openHelpPopup = false;
            }
            if (ImGui.BeginPopup(HelpMenuName))
            {
                ImGui.TextColored(LightGreen, "Chat commands (capitalization does not matter):");
                ImGui.BulletText("\"truth\" \"t\" \"dare\" \"d\" to choose when you're the \"loser\"");
                ImGui.BulletText("\"dch\" \"dealer's choice\" if you want the other to decide");
                ImGui.BulletText("\"coinflip\" \"coin\" to have the plogon decide for you");
                ImGui.BulletText("The keyword set in configuration to mark the truth/dare as valid and go next");
                ImGui.BulletText("And \"prompt\" \"hint\" to get a random prompt based on your N/SFW preferences and choice");
                ImGui.TextColored(Pink, "Also, you can drag the column borders to change their size.");
                ImGui.TextUnformatted("For anything else, just hover the mouse over things.");
                ImGui.EndPopup();
            }
        }
    }
}
