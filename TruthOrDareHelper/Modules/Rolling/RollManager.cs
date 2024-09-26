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
            }

            List<PlayerPair> pairs = new();
            List<PlayerInfo> alreadyTaken = new();
            for (int i = 0; i < pairsToForm; i++)
            {
                PlayerInfo winner = winRolls.First(p => !alreadyTaken.Contains(p.Player)).Player;
                PlayerInfo loser = lossRolls.First(p => !alreadyTaken.Contains(p.Player)).Player;
                pairs.Add(new PlayerPair(winner, loser, DateTime.Now));
                alreadyTaken.Add(winner);
                alreadyTaken.Add(loser);
            }

            return pairs;
        }
    }
}
