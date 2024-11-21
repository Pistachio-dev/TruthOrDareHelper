namespace TruthOrDareHelper.Modules.Prompting.DefaultPrompts
{
    internal interface IPromptCollection
    {
        public string FileName { get; init; }
        public string[] LoadedPromts { get; set; }
        public string[] DefaultPrompts { get; }
    }
}
