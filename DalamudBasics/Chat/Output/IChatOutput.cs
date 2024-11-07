using Dalamud.Game.Text;

namespace DalamudBasics.Chat.Output
{
    public interface IChatOutput
    {
        void WriteChat(string message, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0);

        void SendTell(string message, string playerFullName, string playerHomeWorld, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0);
    }
}
