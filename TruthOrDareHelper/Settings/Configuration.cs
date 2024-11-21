using Dalamud.Game.Text;
using Model;
using System;

namespace TruthOrDareHelper.Settings;

[Serializable]
public class Configuration : IToDConfiguration
{
    public int Version { get; set; } = 0;

    public RollingType RollingType { get; set; } = RollingType.PluginWeightedRng;

    public int SimultaneousPlays { get; set; } = 3;

    public int MaxParticipationStreak { get; set; } = 3;

    public string ConfirmationKeyword { get; set; } = "#nova";

    public bool UseTestData { get; set; } = false;

    public XivChatType DefaultOutputChatType { get; set; } = XivChatType.Party;

    public bool LogOutgoingChatOutput { get; set; } = true;
    public bool LogClientOnlyChatOutput { get; set; } = true;
    public int LimitedChatChannelsMessageDelayInMs { get; set; } = 1500;

    public AskedAcceptedType DefaultTruthAcceptance { get; set; } = AskedAcceptedType.SFW;
    public AskedAcceptedType DefaultDareAcceptance { get; set; } = AskedAcceptedType.SFW;

    public bool MarkPlayers { get; set; } = true;

    public void Save()
    {
        throw new NotImplementedException();
    }
}
