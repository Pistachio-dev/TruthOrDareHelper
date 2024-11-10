using Dalamud.Game.Text.SeStringHandling;
using DalamudBasics.Extensions;
using System.Text.RegularExpressions;

namespace DalamudBasics.Chat.Interpretation.DiceReadingStrategy
{
    public class DiceReadingStrategyEnglish
    {
        public DiceRollType GetRollType(SeString message)
        {           
            var payload1 = message.GetPayload(1);
            if (payload1?.Type == PayloadType.RawText && payload1.GetText().Contains("Random!"))
            {
                return DiceRollType.Dice;
            }

            var payload0 = message.GetPayload(0);
            if (payload0?.Type == PayloadType.RawText && payload0.GetText().Contains("Random!"))
            {
                return DiceRollType.Random;
            }

            return DiceRollType.None;
        }

        public bool TryParseDiceRoll(SeString message, out ChatDiceRoll chatDiceRoll, string rollerFullName)
        {
            chatDiceRoll = new ChatDiceRoll();
            chatDiceRoll.Type = DiceRollType.Dice;
            chatDiceRoll.RollingPlayer = rollerFullName;
            if (message.Payloads.Count != 5)
            {
                return false;
            }

            var regex = new Regex("Random! (?:\\(\\d+-(\\d+)\\))?");
            var match = regex.Match(message.GetPayload(1)?.GetText() ?? string.Empty);
            if (!match.Success)
            {
                return false;
            }

            if (match.Groups[1].Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int maxRange))
                {
                    chatDiceRoll.IsRangeLimited = true;
                    chatDiceRoll.SetRange(1, maxRange);
                }
            }

            if (!int.TryParse(message.GetPayload(3)?.GetText() ?? string.Empty, out int rolledNumber))
            {
                return false;
            }

            chatDiceRoll.RolledNumber = rolledNumber;
            return true;
        }
    }
}
