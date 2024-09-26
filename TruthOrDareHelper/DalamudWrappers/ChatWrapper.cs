using TruthOrDareHelper.DalamudWrappers.Interface;
using static Dalamud.Plugin.Services.IChatGui;

namespace TruthOrDareHelper.DalamudWrappers
{
    public class ChatWrapper : IChatWrapper
    {
        public void AttachMethodToChatMessageReceived(OnMessageDelegate method)
        {
            Plugin.Chat.ChatMessage += method;
        }

        public void SendMessage(string messagePrefix, string message)
        {
            string sanitizedText = ECommons.Automation.Chat.Instance.SanitiseText(message);
            ECommons.Automation.Chat.Instance.SendMessage(messagePrefix + sanitizedText);
        }

        // Print functions are client only. Send functions actually send data to the server.
        public void Print(string message)
        {
            Plugin.Chat.Print(message);
        }

        public void PrintError(string message)
        {
            Plugin.Chat.PrintError(message);
        }
    }
}
