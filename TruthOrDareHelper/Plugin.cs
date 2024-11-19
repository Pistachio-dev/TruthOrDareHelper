using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.Listener;
using DalamudBasics.Configuration;
using DalamudBasics.Debugging;
using DalamudBasics.DependencyInjection;
using DalamudBasics.Interop;
using DalamudBasics.Logging;
using ECommons;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Modules.Rolling;
using TruthOrDareHelper.Settings;
using TruthOrDareHelper.TestData;
using TruthOrDareHelper.Windows;

namespace TruthOrDareHelper;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/tod";

    public readonly WindowSystem WindowSystem = new("TruthOrDareHelper");
    private IServiceProvider serviceProvider;
    private ILogService logService;
    private Configuration configuration;
    private ICommandManager commandManager;

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public ITruthOrDareSession Session { get; set; }

    private static ServiceProvider services { get; set; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this);

        serviceProvider = BuildServiceProvider(pluginInterface);
        logService = serviceProvider.GetRequiredService<ILogService>();
        configuration = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>().GetConfiguration();
        InitializeServices(serviceProvider);

        Session = serviceProvider.GetRequiredService<ITruthOrDareSession>();

        if (configuration.UseTestData)
        {
            Session = Session.AddDummyPlayers().AddRandomSessionParticipation().MakePlayer3BeOnStreak();
        }

        // TODO: remember to attach listener.

        ConfigWindow = new ConfigWindow(logService, serviceProvider);
        MainWindow = new MainWindow(this, logService, serviceProvider);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        commandManager = serviceProvider.GetRequiredService<ICommandManager>();
        commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Launches main ToD window"
        });

        pluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public static T Resolve<T>() where T : notnull
    {
        return services.GetRequiredService<T>();
    }

    public void Dispose()
    {
        serviceProvider.GetRequiredService<HookManager>().Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        commandManager.RemoveHandler(CommandName);
    }

    private IServiceProvider BuildServiceProvider(IDalamudPluginInterface pluginInterface)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAllDalamudBasicsServices<Configuration>(pluginInterface);
        serviceCollection.AddSingleton<StringDebugUtils>();
        serviceCollection.AddSingleton<ITruthOrDareSession, TruthOrDareSession>();
        serviceCollection.AddSingleton<IRollManager, RollManager>();
        serviceCollection.AddSingleton<IToDChatOutput, ToDChatOutput>();

        return serviceCollection.BuildServiceProvider();
    }

    private void InitializeServices(IServiceProvider serviceProvider)
    {
        IFramework framework = serviceProvider.GetRequiredService<IFramework>();
        serviceProvider.GetRequiredService<ILogService>().AttachToGameLogicLoop(framework);
        serviceProvider.GetRequiredService<IChatListener>().InitializeAndRun("[ToD]");
        serviceProvider.GetRequiredService<HookManager>();
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
