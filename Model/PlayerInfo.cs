namespace Model
{
    public class PlayerInfo
    {
        public string FullName { get; set; }
        public List<RoundParticipationRecord> ParticipationRecords { get; set; } = new();
        public ParticipationCounter ParticipationCounter { get; set; } = new();
        public int LastRollResult { get; set; }

        public int Wins => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Winner).Count();
        public int Losses => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Loser).Count();

        public bool IsOnStreak(int streakSize)
        {
            if (!ParticipationRecords.Any())
            {
                return false;
            }

            int streakCounter = 0;
            for (int i = ParticipationRecords.Count - 1; i >= 0; i--)
            {
                if (ParticipationRecords[i].Participation == RoundParticipation.NotParticipating)
                {
                    return false;
                }

                streakCounter++;

                if (streakCounter == streakSize)
                {
                    return true;
                }
            }

            return false;
        }

        public PlayerInfo(string fullName)
        {
            this.FullName = fullName;
        }
    }
}
