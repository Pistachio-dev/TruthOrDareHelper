using Dalamud.Game.Text;

namespace DalamudBasics.Chat.Output
{
    internal class ChatOutputQueuedMessage
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
