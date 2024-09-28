using TruthOrDareHelper.Modules.Chat;

namespace TruthOrDareHelper.Settings
{
    public interface IConfiguration
    {
        ChatChannelType DefaultChatChannel { get; set; }
        RollingType RollingType { get; set; }
        int SimultaneousPlays { get; set; }
        int Version { get; set; }
        int MaxParticipationStreak { get; set; }
        string ConfirmationKeyword { get; set; }
        bool UseTestData { get; set; }

        void Save();
    }
}
