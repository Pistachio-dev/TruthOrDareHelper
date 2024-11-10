using Dalamud.Game.Text;
using System;

namespace DalamudBasics.Chat.Listener
{
    public interface IChatListener
    {
        public event ChatMessageHandler OnChatMessage;

        public delegate void ChatMessageHandler(XivChatType type, string senderFullName, string message, DateTime receivedAt);

        public void InitializeAndRun(string pluginMessageMark);
    }
}
