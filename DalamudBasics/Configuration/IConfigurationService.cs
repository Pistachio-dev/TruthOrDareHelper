namespace DalamudBasics.Configuration
{
    internal interface IConfigurationService<T> where T : IConfiguration, new()
    {
        T GetConfiguration();
        void SaveConfiguration();
    }
}
