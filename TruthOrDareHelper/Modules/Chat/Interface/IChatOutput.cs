namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface IChatOutput
    {
        void WriteChat(string message, ChatChannelType? chatChannel = null);
    }
}