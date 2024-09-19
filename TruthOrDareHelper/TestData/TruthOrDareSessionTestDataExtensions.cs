using FFXIVClientStructs;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.TestData
{
    public static class TruthOrDareSessionTestDataExtensions
    {
        public static TruthOrDareSession AddDummyPlayers(this TruthOrDareSession session)
        {
            for (int i = 1; i < 9; i++)
            {
                string playerName = "Player" + i;
                session.PlayerInfo.Add(playerName, new PlayerInfo(playerName));
            }

            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player1"), session.GetPlayer("Player2")));
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player3"), session.GetPlayer("Player8")));
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player4"), session.GetPlayer("Player7")));

            return session;
        }
    }
}
