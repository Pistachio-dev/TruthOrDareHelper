using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.Rolling
{
    public class Roll
    {
        public PlayerInfo Player { get; set; }
        public int RollResult { get; set; }

        public Roll(PlayerInfo player, int rollResult)
        {
            Player = player;
            RollResult = rollResult;
        }
    }
}
