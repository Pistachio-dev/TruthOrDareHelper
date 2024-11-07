using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAllServices(this IServiceCollection serviceCollection, IDalamudPluginInterface pi)
        {
            return serviceCollection.AddDalamudServices(pi)
                .AddSingleton<IChatOutput, ChatOutput>();                
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
