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

    public int MaxParticipationStreak { get; set; } = 999;

    public string ConfirmationKeyword { get; set; } = "kthxbye";

    public bool UseTestData { get; set; } = false;

    public XivChatType DefaultOutputChatType { get; set; } = XivChatType.Party;

    public bool LogOutgoingChatOutput { get; set; } = true;
    public bool LogClientOnlyChatOutput { get; set; } = true;
    public int LimitedChatChannelsMessageDelayInMs { get; set; } = 1500;

    public AskedAcceptedType DefaultTruthAcceptance { get; set; } = AskedAcceptedType.Any;
    public AskedAcceptedType DefaultDareAcceptance { get; set; } = AskedAcceptedType.Any;

    public bool AllowChangeDecision { get; set; } = false;
    public bool AutoRollOnAllComplete { get; set; } = false;

    public bool MarkPlayers { get; set; } = true;

    public bool ConfirmChallengeChoice { get; set; } = false;

    public bool WriteAcceptedChallenges { get; set; } = false;
}
