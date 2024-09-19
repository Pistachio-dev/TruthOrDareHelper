using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PlayerPair
    {
        public PlayerInfo Winner { get; set; }
        public PlayerInfo? Loser { get; set; }
        public DateTime TimeAnnounced { get; set; }

        public PlayerPair(PlayerInfo winner, PlayerInfo? loser, DateTime? timeAnnouncedUtc = null)
        {
            Winner = winner;
            Loser = loser;
            TimeAnnounced = timeAnnouncedUtc ?? DateTime.UtcNow;
        }
    }
}
