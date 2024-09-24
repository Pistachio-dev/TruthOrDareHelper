namespace Model
{
    public class PlayerInfo
    {
        public string FullName { get; set; }
        public ParticipationCounter TruthParticipation { get; set; }
        public ParticipationCounter DareParticipation { get; set; }
        public List<RoundParticipationRecord> ParticipationRecords { get; set; } = new();

        public int Wins => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Winner).Count();
        public int Losses => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Loser).Count();

        public PlayerInfo(string fullName)
        {
            this.FullName = fullName;
            TruthParticipation = new ParticipationCounter();
            DareParticipation = new ParticipationCounter();
        }
    }
}
