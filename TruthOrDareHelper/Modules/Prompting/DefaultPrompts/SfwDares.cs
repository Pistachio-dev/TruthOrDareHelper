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
        ];
    }
}
