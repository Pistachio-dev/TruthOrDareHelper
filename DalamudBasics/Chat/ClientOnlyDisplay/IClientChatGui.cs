using Dalamud.Game.Text;

namespace DalamudBasics.Chat.ClientOnlyDisplay
{
    public interface IClientChatGui
    {
        void Print(string message);

        void Print(string message, XivChatType chatType);

        void Print(string message, XivChatType chatType, string senderName);

        void PrintError(string message);
    }
}
