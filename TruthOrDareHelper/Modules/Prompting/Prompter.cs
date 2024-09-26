using TruthOrDareHelper.Modules.Prompting.Interface;

namespace TruthOrDareHelper.Modules.Prompting
{
    public class Prompter : IPrompter
    {
        public string GetPrompt(PromptType promptType)
        {
            return $"TODO: This would give a random prompt of type {promptType}";
        }
    }
}
