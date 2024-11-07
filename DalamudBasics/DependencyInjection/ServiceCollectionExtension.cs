using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Listener;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using DalamudBasics.Time;
using Microsoft.Extensions.DependencyInjection;

namespace DalamudBasics.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds all the services in Dalamud and the library to the dependency injection container.
        /// </summary>
        /// <param name="serviceCollection">DI container.</param>
        /// <param name="pi">Interface of the plugin using this library.</param>
        /// <returns></returns>
        public static IServiceCollection AddAllServices<T>(this IServiceCollection serviceCollection, IDalamudPluginInterface pi) where T : IConfiguration, new()
        {
            serviceCollection.AddDalamudServices(pi)
                .AddSingleton<IChatOutput, ChatOutput>()
                .AddSingleton<ITimeUtils, TimeUtils>()
                .AddSingleton<IClientChatGui, ClientChatGui>()
                .AddSingleton<ILogService, LogService>()
                .AddSingleton<ITargetingService, TargetingService>()
                .AddSingleton<IChatListener, ChatListener>();

            serviceCollection.AddSingleton<IConfigurationService<T>, ConfigurationService<T>>();

            return serviceCollection;
        }

        private static IServiceCollection AddDalamudServices(this IServiceCollection services, IDalamudPluginInterface pi)
        {
            AddDalamudService<IDalamudPluginInterface>(services, pi);
            AddDalamudService<ITextureProvider>(services, pi);
            AddDalamudService<ICommandManager>(services, pi);
            AddDalamudService<IPluginLog>(services, pi);
            AddDalamudService<IChatGui>(services, pi);
            AddDalamudService<ITargetManager>(services, pi);
            AddDalamudService<IToastGui>(services, pi);
            AddDalamudService<IClientState>(services, pi);
            AddDalamudService<IPartyList>(services, pi);
            AddDalamudService<ICondition>(services, pi);
            AddDalamudService<IGameInventory>(services, pi);
            AddDalamudService<IObjectTable>(services, pi);
            AddDalamudService<IFramework>(services, pi);

            return services;
        }

        private static void AddDalamudService<T>(IServiceCollection services, IDalamudPluginInterface pi) where T : class
        {
            var wrapper = new DalamudServiceWrapper<T>(pi);
            services.AddSingleton(wrapper.Service);
        }
    }
}
