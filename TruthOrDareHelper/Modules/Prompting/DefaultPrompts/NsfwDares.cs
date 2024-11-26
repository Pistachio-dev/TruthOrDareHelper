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
            "Get naked",
            "You get 7 minutes in heaven with X",
            "Put on the closest thing you have to BSDM gear",
            "Bend over and let the room line up to spank you",
            "Make out with X for Y minutes",
            "Go down on X for Y minutes",
            "You are a noir detective and X just entered into your office. Write the scene",
            "Change Y body part to X size with C+",
            "Open Eorzea Nightlife, roll a dice, and use whichever animation number that you rolled on someone",
            "Sit in the lap of the person next to you and give them a kiss.",
            "Remove a piece of your clothing.",
            "Do a sexual dance in-front of a volunteer for 4 rounds or until your next dare.",
            "Ask the person you are most interested in how you can best please them sexually.",
            "Give a lap dance to someone for 5 minutes.",
            "Change into your more sexual glam.",
            "Sit on the floor in-front of the person you most want to have sex with.",
            "Roll a dice to determine a player and then follow one instruction that they give you.",
            "Roll a dice to determine a player and then shake your ass in-front of them for 5 minutes.",
            "Take off your clothes for 3 rounds.",
            "Put on a gag (Or roleplay it) to mumble your speech for 3 rounds or until your next turn.",
            "Flirt with the person you are most interested in.",
            "Share your last sexual encounter with everyone in as much detail as you are comfortable with.",
            "Pole dance for the next 10 minutes or until your next dare.",
            "Roll a dice to determine a player and then whisper them what you would like to do to them.",
            "Share a lewd gpose of yourself with the group.",
            "Roll a dice to determine a player and then take a lewd gpose with them.",
        ];
    }
}
