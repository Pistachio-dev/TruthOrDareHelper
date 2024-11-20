namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface ICommandRunner
    {
        bool RunRelevantCommand(string sender, string message);
    }
}