namespace Model
{
    public class TruthOrDareSession : ITruthOrDareSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public int Round { get; set; } = 0;

        public Dictionary<string, PlayerInfo> PlayerInfo { get; set; } = new();

        public List<PlayerPair> PlayingPairs { get; set; } = new();

        public PlayerInfo? GetPlayer(string name)
        {
            return PlayerInfo.ContainsKey(name) ? PlayerInfo[name] : null;
        }

        public void AddNewPlayer(string fullName)
        {
            PlayerInfo[fullName] = new PlayerInfo(fullName);
        }

        public void TryRemovePlayer(string fullName)
        {
            if (PlayerInfo.ContainsKey(fullName))
            {
                PlayerInfo.Remove(fullName);
            }
        }

        public bool IsPlayerPlaying(PlayerInfo player)
        {
            return PlayingPairs.FirstOrDefault(p => p.Winner == player || p.Loser == player) != null;
        }

        public bool ArePlayersPaired() => PlayingPairs.Any();
    }
}
