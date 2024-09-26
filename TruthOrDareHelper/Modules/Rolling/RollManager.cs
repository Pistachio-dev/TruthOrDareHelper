using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.Rolling
{
    public class RollManager : IRollManager
    {
        public List<PlayerPair> RollStandard(List<PlayerInfo> players, int maxParticipationStreak, int pairsToForm)
        {
            Random rng = new Random();
            List<PlayerInfo> elegiblePlayers = players.Where(p => !p.IsOnStreak(maxParticipationStreak)).ToList();
            List<Roll> winRolls = elegiblePlayers.Select(p => new Roll(p, rng.Next(100))).OrderBy(r => r.RollResult).ToList();
            List<Roll> lossRolls = elegiblePlayers.Select(p => new Roll(p, rng.Next(100))).OrderBy(r => r.RollResult).ToList();

            if (elegiblePlayers.Count < pairsToForm * 2)
            {
                Plugin.Chat.PrintError($"Not enough players to form {pairsToForm} pairs, reducing amount to {elegiblePlayers.Count / 2}.");
                pairsToForm = elegiblePlayers.Count / 2;
                if (pairsToForm == 0)
                {
                    Plugin.Chat.PrintError($"Not enough playes to form a single pair! Let's lift streak restrictions.");
                    elegiblePlayers = new List<PlayerInfo>(players);
                    pairsToForm = elegiblePlayers.Count / 2;
                }
            }

            List<PlayerPair> pairs = new();
            List<PlayerInfo> alreadyTaken = new();
            for (int i = 0; i < pairsToForm; i++)
            {
                PlayerInfo winner = winRolls.First(p => !alreadyTaken.Contains(p.Player)).Player;
                alreadyTaken.Add(winner);
                PlayerInfo loser = lossRolls.First(p => !alreadyTaken.Contains(p.Player)).Player;
                alreadyTaken.Add(loser);
                pairs.Add(new PlayerPair(winner, loser, DateTime.Now));
            }

            return pairs;
        }

        public PlayerInfo? Reroll(ITruthOrDareSession session)
        {
            List<PlayerInfo> alreadyUsed = new();
            foreach (var pair in session.PlayingPairs)
            {
                alreadyUsed.Add(pair.Winner);
                if (pair.Loser != null)
                {
                    alreadyUsed.Add(pair.Loser);
                }                
            }

            if (alreadyUsed.Count == session.PlayerInfo.Count)
            {
                Plugin.Chat.PrintError($"Can't reroll, all players are already playing");
                return null;
            }

            List<PlayerInfo> elegiblePlayers = session.PlayerInfo.Select(kvp => kvp.Value).Where(p => !alreadyUsed.Contains(p)).ToList();
            return elegiblePlayers[new Random().Next(elegiblePlayers.Count)];
        }
    }
}
