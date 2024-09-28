using Dalamud.Game.Gui.Toast;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private int rollingType;
    private int simultaneousPlays;
    private int maxParticipationStreak;
    private int defaultChatChannel;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("ToD Configuration###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(700, 700);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;

        rollingType = (int)Configuration.RollingType;
        simultaneousPlays = Configuration.SimultaneousPlays;
        maxParticipationStreak = Configuration.MaxParticipationStreak;
        defaultChatChannel = (int)Configuration.DefaultChatChannel;
    }

    public void Dispose()
    { }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
        ImGui.BeginGroup();
        ImGui.TextUnformatted("Write to: "); ImGui.SameLine();
        ImGui.RadioButton("/echo", ref defaultChatChannel, (int)ChatChannelType.Echo); ImGui.SameLine();
        ImGui.RadioButton("/party", ref defaultChatChannel, (int)ChatChannelType.Party); ImGui.SameLine();
        ImGui.RadioButton("/alliance", ref defaultChatChannel, (int)ChatChannelType.Alliance); ImGui.SameLine();
        ImGui.EndGroup();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("What chat channel the plugin will write to.");
        }

        ImGui.InputInt("How many pairs are formed in a round", ref simultaneousPlays);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("For big groups, you can have as many pairs of asker->asked as you want on every round.");
        }

        ImGui.InputInt("Maximum participation streak", ref maxParticipationStreak);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Players that have won or lost these amount of rounds in a row won't roll for the next round");
        }

        //ImGui.BeginGroup();
        //ImGui.TextUnformatted("Player pair rolling type: ");
        //ImGui.RadioButton("Plugin rolls everything", ref defaultChatChannel, (int)ChatChannelType.Echo);
        //ImGui.RadioButton("Plugin writes /dice to chat", ref defaultChatChannel, (int)ChatChannelType.Party);
        //ImGui.RadioButton("Plugin rolls, weighted in favour of idle players", ref defaultChatChannel, (int)ChatChannelType.Alliance);
        //ImGui.EndGroup();
        //if (ImGui.IsItemHovered())
        //{
        //    ImGui.SetTooltip("What way the plugin decides the player pairs.");
        //}

        if (ImGui.Button("Save"))
        {
            if (simultaneousPlays <= 0)
            {
                simultaneousPlays = 1;
            }

            if (maxParticipationStreak <= 0)
            {
                maxParticipationStreak = 1;
            }

            Configuration.RollingType = (RollingType)rollingType;
            Configuration.SimultaneousPlays = simultaneousPlays;
            Configuration.MaxParticipationStreak = maxParticipationStreak;
            Configuration.DefaultChatChannel = (ChatChannelType)defaultChatChannel;
            Configuration.Save();
            //Plugin.Chat.PrintError(PasswordManager.GetCharacterPassword());
            Plugin.ToastGui.ShowNormal("Configuration saved.");
        }
    }
}
