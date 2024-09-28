using Dalamud.Configuration;
using System;
using TruthOrDareHelper.Modules.Chat;

namespace TruthOrDareHelper.Settings;

[Serializable]
public class Configuration : IPluginConfiguration, IConfiguration
{
    public int Version { get; set; } = 0;

    public RollingType RollingType { get; set; } = RollingType.PluginRng;

    public int SimultaneousPlays { get; set; } = 3;

    public int MaxParticipationStreak { get; set; } = 3;

    public ChatChannelType DefaultChatChannel { get; set; } = ChatChannelType.Party;

    public string ConfirmationKeyword { get; set; } = "#nova";

    public bool UseTestData { get; set; } = false;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
