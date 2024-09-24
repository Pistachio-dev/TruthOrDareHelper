namespace TruthOrDareHelper.DalamudWrappers
{
    public class ChatWrapper
    {
        public void SendMessage(string messagePrefix, string message)
        {
            string sanitizedText = ECommons.Automation.Chat.Instance.SanitiseText(message);
            ECommons.Automation.Chat.Instance.SendMessage(messagePrefix + sanitizedText);
        }

        public void PrintError(string message)
        {
            Plugin.Chat.PrintError(message);
        }
    }
}
