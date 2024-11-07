using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Configuration
{
    internal class ConfigurationService<T> : IConfigurationService<T> where T : IConfiguration, new()
    {
        private string fileRoute;
        private T? configuration;

        public ConfigurationService(IDalamudPluginInterface pluginInterface)
        {
            string configDirectory = pluginInterface?.GetPluginConfigDirectory() ?? throw new Exception("Could not retrieve the configuration directory route.");
            this.fileRoute = configDirectory + ".json";
        }

        public T GetConfiguration()
        {
            if (configuration != null)
            {
                return configuration;
            }

            if (!File.Exists(fileRoute))
            {
                configuration = new T();
                SaveConfiguration();
                return configuration;
            }

            string jsonText = File.ReadAllText(fileRoute);
            configuration = JsonConvert.DeserializeObject<T>(jsonText) ?? throw new Exception("Error loading configuration, loaded result is null.");

            return configuration;
        }

        public void SaveConfiguration()
        {
            string jsonText = JsonConvert.SerializeObject(GetConfiguration());
            File.WriteAllText(fileRoute, jsonText);
        }
    }
}
