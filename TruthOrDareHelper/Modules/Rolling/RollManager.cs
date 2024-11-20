using DalamudBasics.Chat.ClientOnlyDisplay;
using Lumina.Excel.Sheets;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthOrDareHelper.Modules.Rolling
{
    public class RollManager : IRollManager
    {
        private readonly IClientChatGui chatGui;

        public RollManager(IClientChatGui chatgui)
        {
            this.chatGui = chatgui;
        }

        public List<PlayerPair> RollStandard(List<PlayerInfo> players, int maxParticipationStreak, int pairsToForm)
        {
            if (players.Count < 2)
            {
                throw new Exception("Can't roll without at least two players");
            }
            Random rng = new Random();
            (var elegiblePlayers, pairsToForm) = GetElegiblePlayers(players, maxParticipationStreak, pairsToForm);
            LinkedList<Roll> rolls = new LinkedList<Roll>(elegiblePlayers.Select(p => new Roll(p, rng.Next(100))).OrderBy(r => r.RollResult));
            foreach (var roll in rolls) { roll.Player.LastRollResult = roll.RollResult; }

            return GeneratePairs(rolls);
        }

        private List<PlayerPair> GeneratePairs(LinkedList<Roll> rolls)
        {
            List<PlayerPair> pairs = new();
            List<LinkedListNode<Roll>> alreadyTaken = new();
            LinkedListNode<Roll>? winnerPointer = rolls.Last;
            LinkedListNode<Roll>? loserPointer = rolls.First;
            while (true)
            {
                while (alreadyTaken.Contains(loserPointer!))
                {
                    loserPointer = loserPointer!.Next;
                    if (loserPointer == winnerPointer) { return pairs; }
                }
                while (alreadyTaken.Contains(winnerPointer!))
                {
                    winnerPointer = winnerPointer!.Previous;
                    if (winnerPointer == loserPointer) { return pairs; }
                }

                alreadyTaken.Add(loserPointer!);
                alreadyTaken.Add(winnerPointer!);
                pairs.Add(new PlayerPair(winnerPointer!.Value.Player, loserPointer!.Value.Player));
            }
        }
        private (List<PlayerInfo> elegiblePlayers, int pairsToForm) GetElegiblePlayers(List<PlayerInfo> players, int maxParticipationStreak, int pairsToForm)
        {
            List<PlayerInfo> elegiblePlayers = players.Where(p => !p.IsOnStreak(maxParticipationStreak)).ToList();
            if (elegiblePlayers.Count < pairsToForm * 2)
            {
                chatGui.PrintError($"Not enough players to form {pairsToForm} pairs, reducing amount to {elegiblePlayers.Count / 2}.");
                pairsToForm = elegiblePlayers.Count / 2;
                if (pairsToForm == 0)
                {
                    chatGui.PrintError($"Not enough playes to form a single pair! Let's lift streak restrictions.");
                    elegiblePlayers = new List<PlayerInfo>(players);
                    pairsToForm = elegiblePlayers.Count / 2;
                }
            }

            return (elegiblePlayers, pairsToForm);
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
                chatGui.PrintError($"Can't reroll, all players are already playing");
                return null;
            }

            List<PlayerInfo> elegiblePlayers = session.PlayerInfo.Select(kvp => kvp.Value).Where(p => !alreadyUsed.Contains(p)).ToList();
            return elegiblePlayers[new Random().Next(elegiblePlayers.Count)];
        }
    }
}
