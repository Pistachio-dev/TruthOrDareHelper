namespace DalamudBasics.Chat.Listener
{
    internal interface IChatListener
    {
        event ChatListener.ChatMessageHandler OnChatMessage;

        void InitializeAndRun(string pluginMessageMark);
    }
}