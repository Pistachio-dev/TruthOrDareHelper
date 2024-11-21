namespace TruthOrDareHelper.Modules.Prompting.DefaultPrompts
{
    public class NsfwTruths : IPromptCollection
    {
        public NsfwTruths(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; init; }
        public string[] LoadedPromts { get; set; } = [];
        public string Tag { get; } = "[NSFW truth prompt]";

        public string[] DefaultPrompts => [
            "NSFW Truth Prompt placeholder 1",
            "NSFW Truth Prompt placeholder 2",
            "NSFW Truth Prompt placeholder 3",
            "NSFW Truth Prompt placeholder 4",
            "NSFW Truth Prompt placeholder 5",
        ];
    }
}
