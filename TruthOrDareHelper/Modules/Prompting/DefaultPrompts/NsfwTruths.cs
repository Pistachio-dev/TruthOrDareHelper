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
            "You have to spend an entire week in a room with someone. Who do you choose?",
            "Who is the last person you kissed?",
            "Who is the last person you had sex with?",
            "What is your wildest sexual encounter?",
            "How many times have you had sex?",
            "Who is the person you are most attracted to here?",
            "Where is the strangest place you've had sex?",
            "What is your dirtiest kink?",
            "What is the longest time you had sex for?",
            "What is the shortest time you had sex for?",
            "What's your worst sexual experience?",
            "What is your ideal sexual partner?",
            "What is a kink you never expected to actually enjoy?",
            "How many times have you had sex in the last week?",
            "Have you ever had a threesome or more?",
            "Name two people you would want to have sex with at the same time.",
            "What is your best sexual experience?",
            "What is your favourite outfit to have sex in?",
            "What do you love your partner to wear during sex?",
            "Where would you most like to have sex with someone?",
            "Describe how you enjoy being pleasured.",
            "Have you ever had sex with anyone here?",
            "Have you ever faked an orgasm?",
        ];
    }
}
