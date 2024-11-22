using DalamudBasics.Configuration;

namespace TruthOrDareHelper.Settings
{
    public interface IToDConfiguration : IConfiguration
    {
        RollingType RollingType { get; set; }
        int SimultaneousPlays { get; set; }
        int Version { get; set; }
        int MaxParticipationStreak { get; set; }
        string ConfirmationKeyword { get; set; }
        bool UseTestData { get; set; }
    }
}
