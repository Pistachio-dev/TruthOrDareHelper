namespace TruthOrDareHelper.Modules.Prompting.Interface
{
    public interface IPrompter
    {
        string GetPrompt(PromptType promptType);
    }
}