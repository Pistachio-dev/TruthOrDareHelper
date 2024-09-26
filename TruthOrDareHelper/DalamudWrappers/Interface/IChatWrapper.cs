namespace TruthOrDareHelper.DalamudWrappers.Interface
{
    public interface IChatWrapper
    {
        void Print(string message);
        void PrintError(string message);
        void SendMessage(string messagePrefix, string message);
    }
}