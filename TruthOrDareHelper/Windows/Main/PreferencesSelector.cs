using Dalamud.Game.Text.SeStringHandling.Payloads;
using DalamudBasics.Extensions;
using DalamudBasics.GUI.Forms;
using ImGuiNET;
using Model;
using System;
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

        private void DrawAcceptedTopicsPopupButton(PlayerInfo player)
        {
            if (ImGui.Button($"##{player.FullName}"))
            {
                playerSelectedForTopicsAcceptedMenu = player;
                openAcceptedTopicsDialogue = true;
            }
            DrawTooltip("Edit SFW/NSFW preferences.");
        }

        private void DrawAcceptedTopicsCell(PlayerInfo player)
        {
            ImGui.BeginChild($"prefsDisplay##{player.FullName}", new Vector2(ImGui.GetFontSize()*15, ImGui.GetFontSize() * 1f));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 0));
            ImGui.TextColored(LightGreen, "T:");
            ImGui.SameLine(0);
            ImGui.TextColored(Yellow, GetAcceptedTopicText(player.AcceptsSfwTruth, player.AcceptsNsfwTruth));
            ImGui.SameLine(0);
            ImGui.TextColored(LightGreen, " D:");
            ImGui.SameLine();
            ImGui.TextColored(Yellow, GetAcceptedTopicText(player.AcceptsSfwDare, player.AcceptsNsfwDare));
            ImGui.PopStyleVar();
            ImGui.EndChild();            
            
            DrawTooltip("T = Truth, D = Dare, S = SFW, N = NSFW, A = Any, ? = None");
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
