using Dalamud.Game.Text.SeStringHandling;
using DalamudBasics.Debugging;
using ECommons.UIHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DalamudBasics.Chat.Interpretation
{
    public static class ChatMessageInterpreter
    {
        public static readonly Regex EnglishDiceRegex = new Regex("Random! (?:\\(\\d+-(\\d+)\\))?");
        public static bool TryParseDiceRoll(SeString message, out ChatDiceRoll chatDiceRoll)
        {
            // Dice rolls have the format of 5 payloads: [symbol], "Random!" or "Random! (1-999)", [symbol], number rolled as text, [symbol]            
            chatDiceRoll = new ChatDiceRoll();
            if (message.Payloads.Count != 5
                || message.Payloads[0].Type != PayloadType.Icon
                || message.Payloads[1].Type != PayloadType.RawText
                || message.Payloads[2].Type != PayloadType.Icon
                || message.Payloads[3].Type != PayloadType.RawText
                || message.Payloads[4].Type != PayloadType.Icon)
            {
                return false;
            }
            string fixedText = message.Payloads[1].GetText();
            Match fixedTextMatch = EnglishDiceRegex.Match(fixedText);
            if (!fixedTextMatch.Success)
            {
                return false;
            }

            if (fixedTextMatch.Groups[1].Success)
            {
                chatDiceRoll.RangeLimited = true;
                if (int.TryParse(fixedTextMatch.Groups[1].Value, out int parsedHighBoundNumber))
                {
                    chatDiceRoll.UpperLimit = parsedHighBoundNumber;
                }
                chatDiceRoll.LowerLimit = 1;
            }
            else
            {
                chatDiceRoll.RangeLimited = false;
            }

            string rolledNumberText = message.Payloads[3].GetText();
            if (int.TryParse(rolledNumberText, out int parsedRolledNumber))
            {
                chatDiceRoll.RolledNumber = parsedRolledNumber;
                return true;
            }

            return false;
        }
    }
}
