namespace Model
{
    public class TruthOrDareSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public int Round = 0;

        public Dictionary<string, PlayerInfo> PlayerInfo { get; set; } = new();

        public List<PlayerPair> PlayingPairs { get; set; } = new();

        public PlayerInfo GetPlayer(string name)
        {
            return PlayerInfo.ContainsKey(name) ? PlayerInfo[name] : throw new Exception("Player not found.");
        }
    }
}
