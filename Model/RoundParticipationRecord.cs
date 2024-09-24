namespace Model
{
    public class RoundParticipationRecord
    {
        public int Id { get; set; }

        public RoundParticipation Participation { get; set; }

        public RoundParticipationRecord(int id, RoundParticipation participation)
        {
            Id = id;
            Participation = participation;
        }
    }
}
