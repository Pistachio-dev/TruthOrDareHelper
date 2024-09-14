using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PlayerInfo
    {
        public string FullName { get; set; }
        public ParticipationCounter TruthParticipation { get; set; }
        public ParticipationCounter DareParticipation { get; set; }        
    }
}
