using DalamudBasics.Extensions;
using DalamudBasics.GUI.Forms;
using ECommons.GameHelpers;
using ImGuiNET;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                factory.DrawCheckbox("SFW", nameof(PlayerInfo.AcceptsSfwTruth));
                factory.DrawCheckbox("NSFW", nameof(PlayerInfo.AcceptsNsfwTruth));
                ImGui.EndGroup();
                ImGui.Separator();
                ImGui.BeginGroup();
                ImGui.TextColored(Pink, "Accepted dares");
                factory.DrawCheckbox("SFW", nameof(PlayerInfo.AcceptsSfwDare));
                factory.DrawCheckbox("NSFW", nameof(PlayerInfo.AcceptsNsfwDare));
                ImGui.EndGroup();

                ImGui.EndPopup();
            }
        }
    }
}
