namespace Model
{
    public class PlayerPair
    {
        public PlayerInfo Winner { get; set; }
        public PlayerInfo? Loser { get; set; }
        public DateTime TimeAnnounced { get; set; }
        public ChallengeType ChallengeType { get; set; } = ChallengeType.None;
        public bool Done { get; set; } = false;

        public PlayerPair(PlayerInfo winner, PlayerInfo? loser, DateTime? timeAnnouncedUtc = null)
        {
            Winner = winner;
            Loser = loser;
            TimeAnnounced = timeAnnouncedUtc ?? DateTime.UtcNow;
        }
    }
}
