using Dalamud.Plugin.Services;

namespace TruthOrDareHelper.DalamudWrappers.Interface
{
    public interface IChatWrapper
    {
        void AttachMethodToChatMessageReceived(IChatGui.OnMessageDelegate method);
        void Print(string message);
        void PrintError(string message);
        void SendMessage(string messagePrefix, string message);
    }
}
