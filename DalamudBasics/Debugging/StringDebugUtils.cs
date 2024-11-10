using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using DalamudBasics.Chat.Interpretation;
using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Debugging
{
    public class StringDebugUtils
    {
        private readonly ILogService logService;

        public StringDebugUtils(ILogService logService)
        {
            this.logService = logService;
        }

        public void DumpSeString(SeString s)
        {
            int counter = 0;
            foreach (Payload payload in s.Payloads)
            {
                string embeddedInfoType = payload.Type.ToString();
                string text = "Unreadable";
                if (payload is ITextProvider)
                {
                    ITextProvider textProvider = (ITextProvider)payload;
                    text = textProvider.Text;
                }

                string output = $"Payload {counter} Type: {embeddedInfoType} Text: \"{text}\"";
                logService.Info(output);
                counter++;
            }
        }

        public void DumpAllReceivedMessages(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            logService.Info($"Type: {type} Timestamp: {timestamp} IsHandled: {isHandled}");
            logService.Info("Sender SeString dump---------------------------");
            DumpSeString(sender);
            logService.Info("Message SeString dump---------------------------");
            DumpSeString(message);
        }

        public void TestDiceRollParsing(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!ChatMessageInterpreter.TryParseDiceRoll(message, out ChatDiceRoll result))
            {
                logService.Info("No roll detected");
                return;
            }

            if (result.RangeLimited)
            {
                logService.Info($"Roll read: {result.RolledNumber} ({result.LowerLimit} to {result.UpperLimit})");
                
                return;
            }

            logService.Info($"Roll read: {result.RolledNumber}");

        }
    }
}
