namespace Model
{
    public class PlayerInfo
    {
        public string FullName { get; set; }
        public List<RoundParticipationRecord> ParticipationRecords { get; set; } = new();
        public ParticipationCounter ParticipationCounter { get; set; } = new();
        public int LastRollResult { get; set; }
        public bool AcceptsSfwTruth { get; set; }
        public bool AcceptsNsfwTruth { get; set; }
        public bool AcceptsSfwDare { get; set; }
        public bool AcceptsNsfwDare { get; set; }

        public int Wins => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Winner).Count();
        public int Losses => ParticipationRecords.Where(x => x.Participation == RoundParticipation.Loser).Count();

        public PlayerInfo(string fullName, AskedAcceptedType truthPreferences, AskedAcceptedType darePreferences)
        {
            this.FullName = fullName;
            if (truthPreferences is AskedAcceptedType.SFW or AskedAcceptedType.Any)
            {
                AcceptsSfwTruth = true;
            }
            if (truthPreferences is AskedAcceptedType.NSFW or AskedAcceptedType.Any)
            {
                AcceptsNsfwTruth = true;
            }
            if (darePreferences is AskedAcceptedType.SFW or AskedAcceptedType.Any)
            {
                AcceptsSfwDare = true;
            }
            if (darePreferences is AskedAcceptedType.NSFW or AskedAcceptedType.Any)
            {
                AcceptsNsfwDare = true;
            }
        }

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


    }
}
