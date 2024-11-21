using Model;
using System;

namespace TruthOrDareHelper.TestData
{
    public static class TruthOrDareSessionTestDataExtensions
    {
        public static ITruthOrDareSession AddDummyPlayers(this ITruthOrDareSession session)
        {
            for (int i = 1; i < 9; i++)
            {
                string playerName = "Player" + i;
                session.PlayerData.Add(playerName, new PlayerInfo(playerName, AskedAcceptedType.NSFW, AskedAcceptedType.Any));
            }

            return session;
        }

        public static ITruthOrDareSession AddDummyPairs(this ITruthOrDareSession session)
        {
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player1"), session.GetPlayer("Player2")));
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player3"), session.GetPlayer("Player8")));
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer("Player4"), session.GetPlayer("Player7")));

            return session;
        }

        public static ITruthOrDareSession AddRandomSessionParticipation(this ITruthOrDareSession session)
        {
            Random rng = new Random();
            foreach (var player in session.PlayerData.Values)
            {
                for (int i = 0; i < 8; i++)
                {
                    RoundParticipation type;
                    int random = rng.Next(10);
                    if (random < 2)
                    {
                        type = RoundParticipation.Winner;
                    }
                    else if (random >= 2 && random < 4)
                    {
                        type = RoundParticipation.Loser;
                    }
                    else
                    {
                        type = RoundParticipation.NotParticipating;
                    }

                    player.ParticipationRecords.Add(new RoundParticipationRecord(i, type));
                }
            }

            return session;
        }

        public static ITruthOrDareSession MacalaniaAndMe(this ITruthOrDareSession session)
        {
            string name1 = "Pistachio Herald@Omega";
            string name2 = "Macalania Nut@Louisoix";
            session.AddNewPlayer(name1, AskedAcceptedType.Any, AskedAcceptedType.Any);
            session.AddNewPlayer(name2, AskedAcceptedType.Any, AskedAcceptedType.Any);
            session.PlayingPairs.Add(new PlayerPair(session.GetPlayer(name1)!, session.GetPlayer(name2)));

            return session;
        }

        public static ITruthOrDareSession MakePlayer3BeOnStreak(this ITruthOrDareSession session)
        {
            session.PlayerData["Player3"].ParticipationRecords.Add(new RoundParticipationRecord(2342, RoundParticipation.Winner));
            session.PlayerData["Player3"].ParticipationRecords.Add(new RoundParticipationRecord(2343, RoundParticipation.Winner));
            session.PlayerData["Player3"].ParticipationRecords.Add(new RoundParticipationRecord(2345, RoundParticipation.Loser));

            return session;
        }
    }
}
