namespace Model
{
    public class TruthOrDareSession : ITruthOrDareSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public int Round { get; set; } = 0;

        public Dictionary<string, PlayerInfo> PlayerData { get; set; } = new();

        public List<PlayerPair> PlayingPairs { get; set; } = new();

        public PlayerInfo? GetPlayer(string name)
        {
            return PlayerData.ContainsKey(name) ? PlayerData[name] : null;
        }

        public void AddNewPlayer(string fullName, AskedAcceptedType truthPrefs, AskedAcceptedType darePrefs)
        {
            PlayerData[fullName] = new PlayerInfo(fullName, truthPrefs, darePrefs);
        }

        public void TryRemovePlayer(string fullName)
        {
            if (PlayerData.ContainsKey(fullName))
            {
                PlayerData.Remove(fullName);
            }
        }

        public bool IsPlayerPlaying(PlayerInfo player)
        {
            return PlayingPairs.FirstOrDefault(p => p.Winner == player || p.Loser == player) != null;
        }

        public bool ArePlayersPaired() => PlayingPairs.Any();

        public void Reset()
        {
            SessionId = Guid.NewGuid();
            Round = 0;
            PlayerData.Clear();
            PlayingPairs.Clear();
        }
    }
}
