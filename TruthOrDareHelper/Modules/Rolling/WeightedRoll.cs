using Model;
using System;

namespace TruthOrDareHelper.Modules.Rolling
{
    public class WeightedRoll : Roll
    {
        private readonly int numberOfPlayers;
        
        public WeightedRoll(PlayerInfo player, int numberOfPlayers) : base(player)
        {
            Player = player;
            this.numberOfPlayers = numberOfPlayers;
            RollResult = CalculateWeightedRoll();
        }

        // The calculation is: (winPercentage / (1 / numberOfPlayers) * (how much we'd have to add/remove to get the lowest/highest score guaranteed);        
        private int CalculateWeightedRoll()
        {
            int naturalRoll = rng.Next(1, RollExclusiveCeiling);            
            double expectedParticipationRateForRole = 1f / numberOfPlayers;
            double startingWeight = CalculateStartingWeight();
            if (naturalRoll >= RollExclusiveCeiling / 2)
            {
                double winPercentage = Player.ParticipationCounter.Wins / (double)Player.ParticipationCounter.Total;
                double weight = CalculateWeight(winPercentage, expectedParticipationRateForRole, naturalRoll);
                return (int)(naturalRoll - weight);
            }
            if (naturalRoll < RollExclusiveCeiling / 2)
            {
                double lossPercentage = Player.ParticipationCounter.Losses / (double)Player.ParticipationCounter.Total;
                double weight = CalculateWeight(lossPercentage, expectedParticipationRateForRole, RollExclusiveCeiling - naturalRoll);
                return (int)(naturalRoll + weight);
            }

            return naturalRoll;
        }

        private double CalculateWeight(double actualPercentage, double expectedPercentage, int maxWeight)
        {
            return Math.Abs((actualPercentage - expectedPercentage) / expectedPercentage) * maxWeight;
        }

        // This weight allows this to not immediately give a 100% chance to people that joins, since they'll have 0 participation rate.
        private double CalculateStartingWeight()
        {
            int roundsBeforeGuaranteedRole = (int)(numberOfPlayers * 1.5);
            int cappedRoundsParticipated = Math.Min(Player.ParticipationCounter.Total, roundsBeforeGuaranteedRole);
            return cappedRoundsParticipated/Player.ParticipationCounter.Total;
        }
    }
}
