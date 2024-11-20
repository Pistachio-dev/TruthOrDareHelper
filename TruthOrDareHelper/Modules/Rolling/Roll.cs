using Model;
using System;

namespace TruthOrDareHelper.Modules.Rolling
{
    public class Roll
    {
        public const int RollExclusiveCeiling = 1000;
        public static Random rng = new Random();

        public PlayerInfo Player { get; set; }
        public virtual int RollResult { get; set; }

        public Roll(PlayerInfo player)
        {
            Player = player;
            RollResult = rng.Next(RollExclusiveCeiling);
        }
    }
}
