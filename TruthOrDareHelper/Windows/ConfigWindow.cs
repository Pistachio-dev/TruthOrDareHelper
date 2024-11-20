using Dalamud.Game.Text;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Forms;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Numerics;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Windows;

public class ConfigWindow : PluginWindowBase, IDisposable
{
    private IConfigurationService<Configuration> configurationService;
    private IClientChatGui chatGui;
    private ImGuiFormFactory<Configuration> formFactory;
    private Configuration configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(ILogService logService, IServiceProvider serviceProvider) : base(logService, "ToD Configuration")
    {
        Size = new Vector2(700, 700);
        SizeCondition = ImGuiCond.Always;
        this.configurationService = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>();
        this.chatGui = serviceProvider.GetRequiredService<IClientChatGui>();
        this.formFactory = new ImGuiFormFactory<Configuration>(() => configurationService.GetConfiguration(), (data) => configurationService.SaveConfiguration());
        configuration = this.configurationService.GetConfiguration();
    }

    public void Dispose()
    { }

    public override void PreDraw()
    {
    }

    protected override void SafeDraw()
    {
        DrawSectionHeader("Chat");
        ImGui.BeginGroup();
        ImGui.TextUnformatted("Write to: ");
        formFactory.DrawUshortRadio(nameof(Configuration.DefaultOutputChatType), sameLine: true,
            [("/echo", (ushort)XivChatType.Echo, null),
                ("/party", (ushort)XivChatType.Party, null),
                ("/alliance", (ushort)XivChatType.Alliance, null),
                ("/say", (ushort)XivChatType.Say, null)]);

        ImGui.EndGroup();
        DrawTooltip("What chat channel the plugin will write to.");

        ImGui.TextUnformatted("Dice rolling type");
        formFactory.DrawRadio(nameof(Configuration.RollingType), sameLine: true,
            [("Auto normal", (int)RollingType.PluginRng, "The plugin does the rolls instantly, every number has the same probability."),
            ("Auto weighted", (int)RollingType.PluginWeightedRng, "The plugin does the rolls instantly, and people that has participated less than average get progressively higher chances of being selected")
            ]);

        DrawSectionHeader("Game");
        formFactory.AddValidationText(formFactory.DrawIntInput("How many pairs are formed in a round", nameof(Configuration.SimultaneousPlays), EnforcePositiveInt));
        DrawTooltip("For big groups, you can have as many pairs of asker->asked as you want on every round.");

        formFactory.AddValidationText(formFactory.DrawIntInput("Maximum participation streak", nameof(Configuration.MaxParticipationStreak), EnforcePositiveInt));
        DrawTooltip("Players that have won or lost these amount of rounds in a row won't roll for the next round");

        formFactory.DrawTextInput("Confirmation keyword", nameof(Configuration.ConfirmationKeyword), 50);
        DrawTooltip("If the pair winner says this word, it is considered the answer was valid and the next roll is done automatically.");

        DrawSectionHeader("Testing");
        formFactory.DrawCheckbox("Use test data", nameof(Configuration.UseTestData));
        DrawTooltip("Starts the plugin with some dummy data. Only for testing the plugin.");
    }

    private string? EnforcePositiveInt(int number)
    {
        return number >= 0 ? null : "Number must be positive";
    }

    private void DrawSectionHeader(string title)
    {
        ImGui.Separator();
        ImGui.TextUnformatted(title);
    }
}
