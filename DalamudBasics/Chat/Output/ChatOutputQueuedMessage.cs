using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Chat.Output
{
    public class ChatOutputQueuedMessage
    {
        public string Message { get; set; }

        public XivChatType? ChatChannel { get; set; }

        public int SpacingBeforeInMs { get; set; }


        public ChatOutputQueuedMessage(string message, XivChatType? chatOutputType = null, int spacingBeforeInMs = 0)
        {
            Message = message;
            this.ChatChannel = chatOutputType;
            SpacingBeforeInMs = spacingBeforeInMs;
        }
    }
}
