using DalamudBasics.Extensions;
using DalamudBasics.GUI.Forms;
using ImGuiNET;
using Model;
using System.Numerics;

namespace TruthOrDareHelper.Windows.Main
{
    public partial class MainWindow
    {
        private const string AcceptedTopicsPopupName = "Accepted Topics";
        private PlayerInfo? playerSelectedForTopicsAcceptedMenu = null;
        private bool openAcceptedTopicsDialogue = false;
        private ImGuiFormFactory<PlayerInfo> acceptedTopicsFormFactory;

        private void InitializeFormFactory()
        {
            acceptedTopicsFormFactory = new ImGuiFormFactory<PlayerInfo>(() => playerSelectedForTopicsAcceptedMenu!, (player) => { });
        }

        private void DrawAcceptedTopicsCell(PlayerInfo player)
        {
            string text = $"T: {GetAcceptedTopicText(player.AcceptsSfwTruth, player.AcceptsNsfwTruth)} D: {GetAcceptedTopicText(player.AcceptsSfwDare, player.AcceptsNsfwDare)}";
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
            if (ImGui.Button($"{text}##{player.FullName}")){
                playerSelectedForTopicsAcceptedMenu = player;
                openAcceptedTopicsDialogue = true;
            }
            ImGui.PopStyleColor();
            DrawTooltip("Click to edit. T = Truth, D = Dare, S = SFW, N = NSFW, A = Any, ? = None");
        }

        private string GetAcceptedTopicText(bool sfwFlag, bool nsfwFlag)
        {
            if (sfwFlag)
            {
                if(nsfwFlag)
                {
                    return "A";
                }

                return "S";
            }

            if (nsfwFlag)
            {
                return "N";
            }

            return "?";            
        }

        private void DrawPlayerAcceptedTopicsPopup()
        {
            if (openAcceptedTopicsDialogue)
            {
                openAcceptedTopicsDialogue = false;
                ImGui.OpenPopup(AcceptedTopicsPopupName);
            }

            var player = playerSelectedForTopicsAcceptedMenu;
            var factory = acceptedTopicsFormFactory;
            if (ImGui.BeginPopup($"{AcceptedTopicsPopupName}"))
            {
                ImGui.TextColored(Yellow, $"{player?.FullName.GetFirstName() ?? "Someone"}'s preferences:");
                ImGui.BeginGroup();
                ImGui.TextColored(LightGreen, "Accepted truths");
                factory.DrawCheckbox("SFW##Truth", nameof(PlayerInfo.AcceptsSfwTruth));
                factory.DrawCheckbox("NSFW##Truth", nameof(PlayerInfo.AcceptsNsfwTruth));
                ImGui.EndGroup();
                ImGui.Separator();
                ImGui.BeginGroup();
                ImGui.TextColored(Pink, "Accepted dares");
                factory.DrawCheckbox("SFW##Dare", nameof(PlayerInfo.AcceptsSfwDare));
                factory.DrawCheckbox("NSFW##Dare", nameof(PlayerInfo.AcceptsNsfwDare));
                ImGui.EndGroup();

                ImGui.EndPopup();
            }
        }
    }
}
