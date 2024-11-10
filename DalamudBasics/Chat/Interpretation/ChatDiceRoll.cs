using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Chat.Interpretation
{
    public class ChatDiceRoll
    {
        public int RolledNumber { get; set; }
        public bool RangeLimited { get; set; }
        public int LowerLimit { get; set; } = -1;
        public int UpperLimit { get; set; } = -1;
    }
}
