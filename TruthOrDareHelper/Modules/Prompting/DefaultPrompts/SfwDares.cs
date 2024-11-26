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
        public string Tag { get; } = "[SFW dare prompt]";

        public string[] DefaultPrompts => [
            "Show us your funniest mod",
            "Talk like Urianger for the next two rounds",
            "Speak in UwU tongue for the next two rounds",
            "You've been kidnapped and your captor orders you to write the ransom letter with letters cut out from magazines. Here you have them . Write it",
            "Sit on someone's lap",
            "Throw away whatever is in your inventory slot nºX",
            "Talk like you're a drill seargeant for two rounds",
            "Your E key has been confiscated. Write without using it for the next two rounds",
            "Try to cosplay character X using Glamourer. You have Y minutes.",
            "Write a love letter to <person/character>",
            "You're now one of your retainers. Write your resignation letter.",
            "Speak like a pirate for two rounds",
            "Do your best attempt at playing (some known melody or riff) as a bard, manually",
            "Change your race to X for 2 rounds",
            "Become a bald white roegadyn man for 2 rounds",
            "Use SimpleHeels to bury yourself up to the neck, and communicate using only the word \"Digglet\" for one round",
            "Roleplay as a Limsan pirate until your next dare.",
            "Speak only in FF14 Story Quotes until your next dare.",
            "Dance until your next dare.",
            "Tell the worst joke you know.",
            "Change your glam to something you haven't used in a long time.",
            "Show us your worst glam.",
            "Take a silly gpose and share it with the group.",
            "Use only gobbiespeak until your next dare.",
            "End every sentence with \"yes, yes\" for a few rounds.",
            "Stand on the highest object on the room.",
            "Do a performance for two minutes to your best ability.",
            "Roll a dice to determine another player and then become their most supportive cheerleader until your next dare.",
            "Roll a dice to determine another player and give them your best compliment about them.",
            "Speak like a fae until your next dare.",
        ];
    }
}
