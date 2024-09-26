using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
