using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using TruthOrDareHelper.Configuration;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public RollingType RollingType { get; set; } = RollingType.PluginRng;

    public int SimultaneousPlays { get; set; } = 1;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
