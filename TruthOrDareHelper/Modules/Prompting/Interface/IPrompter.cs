namespace TruthOrDareHelper.Modules.Prompting.Interface
{
    public interface IPrompter
    {
        string GetPrompt(bool useSfwTruths, bool useNsfwTruths, bool useSfwDares, bool useNsfwDares);

        string GetStatsString();

        void LoadPromptsToMemory();

        void OpenFolder();

        void SeedIfNeeded();
    }
}
