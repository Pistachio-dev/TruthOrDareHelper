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
            "What's your biggest fantasy?",
            "What's your least weird kink?",
            "What's your weirdest kink?",
            "You find X tied to your bed. What do you do?",
            "At what size of X does it stop being hot and starts being weird for you?",
            "Fuck marry kill, (this group/this room/x group of characters)",
            "How many people have you fucked in your way here?",
            "What's your bodycount?",
            "Hottest sexual encounter?",
            "Weirdest sexual encounter?",
            "Do you have any \"hear me out\" character?",
            "When was your last ERP session?",
            "You have to spend an entire week in a room with someone. Who do you choose?"
        ];
    }
}
