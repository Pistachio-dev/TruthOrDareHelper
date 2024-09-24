using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
