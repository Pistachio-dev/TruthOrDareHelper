namespace TruthOrDareHelper.Modules.Prompting.DefaultPrompts
{
    public class NsfwDares : IPromptCollection
    {
        public NsfwDares(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; init; }
        public string[] LoadedPromts { get; set; } = [];

        public string[] DefaultPrompts => [
            "NSFW Dare Prompt placeholder 1",
            "NSFW Dare Prompt placeholder 2",
            "NSFW Dare Prompt placeholder 3",
            "NSFW Dare Prompt placeholder 4",
            "NSFW Dare Prompt placeholder 5",
        ];
    }
}
