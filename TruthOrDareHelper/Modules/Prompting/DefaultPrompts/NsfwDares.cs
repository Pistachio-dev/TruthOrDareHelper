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
        public string Tag { get; } = "[NSFW dare prompt]";

        public string[] DefaultPrompts => [
            "Give X a lap dance",
            "Get naked",
            "You get 7 minutes in heaven with X",
            "Put on the closest thing you have to BSDM gear",
            "Bend over and let the room line up to spank you",
            "Make out with X for Y minutes",
            "Go down on X for Y minutes",
            "You are a noir detective and X just entered into your office. Write the scene",
            "Change Y body part to X size with C+",
            "Open Eorzea Nightlife, roll a dice, and use whichever animation number that you rolled on someone",
        ];
    }
}
