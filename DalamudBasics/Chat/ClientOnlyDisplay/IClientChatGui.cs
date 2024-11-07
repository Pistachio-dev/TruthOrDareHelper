using Dalamud.Plugin.Services;

namespace DalamudBasics.Chat.ClientOnlyDisplay
{
    public interface IClientChatGui
    {
        void AttachMethodToChatMessageReceived(IChatGui.OnMessageDelegate method);

        void Print(string message);

        void PrintError(string message);

        void SendMessage(string messagePrefix, string message);
    }
}
