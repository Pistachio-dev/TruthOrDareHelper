
namespace Model
{
    public interface ITruthOrDareSession
    {
        Dictionary<string, PlayerInfo> PlayerInfo { get; set; }
        List<PlayerPair> PlayingPairs { get; set; }
        Guid SessionId { get; set; }
        public int Round { get; set; }

        void AddNewPlayer(string fullName);
        bool ArePlayersPaired();
        PlayerInfo? GetPlayer(string name);
        bool IsPlayerPlaying(PlayerInfo player);
        void TryRemovePlayer(string fullName);
    }
}
