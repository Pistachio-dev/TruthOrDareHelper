using Dalamud.Game.Text;
using Dalamud.Plugin.Services;

namespace DalamudBasics.Chat.Output
{
    public interface IChatOutput
    {
        void WriteChat(string message, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0);

        void SendTell(string message, string playerFullName, string playerHomeWorld, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0);
        void AttachToGameLogicLoop(IFramework framework);
    }
}
