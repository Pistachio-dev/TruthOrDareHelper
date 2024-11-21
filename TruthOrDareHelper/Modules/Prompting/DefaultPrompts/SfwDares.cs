namespace TruthOrDareHelper.Modules.Prompting.DefaultPrompts
{
    public class SfwDares : IPromptCollection
    {
        public SfwDares(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; init; }
        public string[] LoadedPromts { get; set; } = [];

        public string[] DefaultPrompts => [
            "SFW Dare Prompt placeholder 1",
            "SFW Dare Prompt placeholder 2",
            "SFW Dare Prompt placeholder 3",
            "SFW Dare Prompt placeholder 4",
            "SFW Dare Prompt placeholder 5",
        ];
    }
}
