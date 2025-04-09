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
    private readonly Vector4 lightGreen = new Vector4(162 / 255f, 1, 153 / 255f, 1);

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

        DrawSectionHeader("Game");
        ImGui.TextUnformatted("Dice rolling type");
        formFactory.DrawRadio(nameof(Configuration.RollingType), sameLine: true,
            [("Auto normal", (int)RollingType.PluginRng, "The plugin does the rolls instantly, every number has the same probability."),
                ("Auto weighted", (int)RollingType.PluginWeightedRng, "The plugin does the rolls instantly, and people that has participated less than average get progressively higher chances of being selected")
            ]);

        formFactory.AddValidationText(formFactory.DrawIntInput("How many pairs are formed in a round", nameof(Configuration.SimultaneousPlays), EnforcePositiveInt));
        DrawTooltip("For big groups, you can have as many pairs of asker->asked as you want on every round.");

        formFactory.AddValidationText(formFactory.DrawIntInput("Maximum participation streak", nameof(Configuration.MaxParticipationStreak), EnforcePositiveInt));
        DrawTooltip("Players that have participated on either role this amount of rounds in a row won't roll for the next round");

        formFactory.DrawTextInput("Confirmation keyword", nameof(Configuration.ConfirmationKeyword), 50);
        DrawTooltip("If the pair winner says this word, it is considered the answer was valid and the next roll is done automatically.");

        formFactory.DrawCheckbox("Put marks on players", nameof(Configuration.MarkPlayers));
        DrawTooltip("If checked, party markers will be applied to playes depending on their role");


        formFactory.DrawCheckbox("Write to chat when someone picks truth or dare", nameof(Configuration.ConfirmChallengeChoice));
        DrawTooltip("If checked, it will print a confirmation message when a choice is detected.");

        formFactory.DrawCheckbox("Allow changing the decision after it's made", nameof(Configuration.AllowChangeDecision));
        DrawTooltip("If checked, players can choose \"truth\" or \"dare\" again after they have already chosen, changing it.");
        
        formFactory.DrawCheckbox("Automatically roll when all are done", nameof(Configuration.AutoRollOnAllComplete));
        DrawTooltip("If checked, when all pairs of player have validated the truth/dare, the plugin will immediately roll the next round.");

        DrawSectionHeader("Default player preferences");
        ImGui.BeginGroup();
        formFactory.DrawComboBox("Truth", nameof(Configuration.DefaultTruthAcceptance), ["None", "SFW only", "NSFW only", "Both SFW and NSFW"]);
        formFactory.DrawComboBox("Dare", nameof(Configuration.DefaultDareAcceptance), ["None", "SFW only", "NSFW only", "Both SFW and NSFW"]);
        ImGui.EndGroup();
        DrawTooltip("Helps you keep track of what kind of challenges a player accepts/wants. It also determines which kind of prompts they get.");

        formFactory.DrawCheckbox("Print player preferences when they get picked to play", nameof(Configuration.AutoRollOnAllComplete));
        DrawTooltip("If checked, it will say which kind of truths and dares the player accepts (SFW/NSFW).");

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
        ImGui.TextColored(lightGreen,title);
    }
}
