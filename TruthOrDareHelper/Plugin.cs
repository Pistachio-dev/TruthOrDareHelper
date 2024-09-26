using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Microsoft.Extensions.DependencyInjection;
using Model;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.DalamudWrappers.Interface;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Prompting;
using TruthOrDareHelper.Modules.Prompting.Interface;
using TruthOrDareHelper.Modules.Targeting;
using TruthOrDareHelper.Modules.Targeting.Interface;
using TruthOrDareHelper.Modules.TimeKeeping;
using TruthOrDareHelper.Modules.TimeKeeping.Interface;
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

    public ITruthOrDareSession Session { get; set; }

    private static ServiceProvider services { get; set; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this);

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        services = DependencyInjectionSetup(Configuration);

        Session = Resolve<ITruthOrDareSession>().AddDummyPlayers().AddRandomSessionParticipation();
       
        // TODO: remember to attach listener.

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

    public static T Resolve<T>() where T: notnull
    {
        return services.GetRequiredService<T>();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private ServiceProvider DependencyInjectionSetup(Configuration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITruthOrDareSession, TruthOrDareSession>();
        services.AddSingleton<IChatWrapper, ChatWrapper>();
        services.AddSingleton<ILogWrapper, LogWrapper>();
        services.AddSingleton<ITargetWrapper, TargetWrapper>();
        services.AddSingleton<IChatListener, ChatListener>();
        services.AddSingleton<IChatOutput, ChatOutput>();
        services.AddSingleton<IPrompter, Prompter>();
        services.AddSingleton<ITargetingHandler, TargetingHandler>();
        services.AddSingleton<ITimeKeeper, TimeKeeper>();
        services.AddSingleton<IConfiguration>((services) => configuration);

        return services.BuildServiceProvider();
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
