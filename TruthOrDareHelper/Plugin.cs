using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Model;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Modules.Targeting;
using TruthOrDareHelper.Settings;
using TruthOrDareHelper.TestData;
using TruthOrDareHelper.Windows;

namespace TruthOrDareHelper;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; } = null!;
    [PluginService] public static ITargetManager TargetManager { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;




    private const string CommandName = "/tod";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("TruthOrDareHelper");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public TruthOrDareSession Session { get; set; }
    public TargetManager targetManager { get; set; }
    public ChatOutput chatOutput { get; set; }
    public ChatListener chatListener { get; set; }
    public LogWrapper logWrapper { get; set; }
    public ChatWrapper chatWrapper { get; set; }
    public TargetWrapper targetWrapper { get; set; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this);

        Session = new TruthOrDareSession().AddDummyPlayers().AddRandomSessionParticipation();

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        logWrapper = new LogWrapper();
        chatWrapper = new ChatWrapper();
        targetWrapper = new TargetWrapper();
        targetManager = new TargetManager(logWrapper, targetWrapper);
        chatOutput = new ChatOutput(Configuration, chatWrapper, logWrapper);
        chatListener = new ChatListener();
        chatListener.AttachListener();
        // you might normally want to embed resources and load them from the manifest stream

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();

    public void ToggleMainUI() => MainWindow.Toggle();
}