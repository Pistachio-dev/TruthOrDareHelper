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
            "What’s your biggest regret?",
            "What is your biggest fear?",
            "What is your biggest achievement in the game?",
            "Who would you most want to spend an evening with?",
            "What are you too scared to try?",
            "If you could remove one job from the game what would It be?",
            "What job would you want most added to the game?",
            "If you were forced to take a fantasia to a different race which one would it be?",
            "What was your best moment in the game?",
            "Have you ever been blacklisted?",
            "What Is your favourite place to hangout?",
            "You're in a desert, walking along in the sand, when all of a sudden you look down and see a tortoise, crawling towards you. You reach down and you flip the tortoise over on its back." +
            "The tortoise lays on its back, its belly baking in the hot sun, beating its legs trying to turn itself over, but it can't. Not without your help. But you're not helping. Why is that?",
            "What's your honest opinion on <someone>",
            "Have you ever hated a game but kept playing to see it through?",
            "You receive 100 million dollars, with the condition that they are used to make a game. What do you make?",
            "Ever considered writing a book?",
            "Is there a topic that you find interesting but can't really understand?",
            "Is there something you have put too much time on and are embarrased to admit it?",
            "Google your character name, what's the first result in image search?",
        ];
    }
}
