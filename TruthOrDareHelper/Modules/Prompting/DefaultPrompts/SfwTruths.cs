namespace TruthOrDareHelper.Modules.Prompting.DefaultPrompts
{
    public class SfwTruths : IPromptCollection
    {
        public SfwTruths(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; init; }
        public string[] LoadedPromts { get; set; } = [];
        public string Tag { get; } = "[SFW truth prompt]";

        public string[] DefaultPrompts => [
            "SFW Truth Prompt placeholder 1",
            "SFW Truth Prompt placeholder 2",
            "SFW Truth Prompt placeholder 3",
            "SFW Truth Prompt placeholder 4",
            "SFW Truth Prompt placeholder 5",
        ];
    }
}
