using Dalamud.Game.Text.SeStringHandling;

namespace DalamudBasics.Debugging
{
    public static class PayloadExtensions
    {
        public static string GetText(this Payload payload)
        {
            if (payload == null || !(payload is ITextProvider))
            {
                return string.Empty;
            }

            return (payload as ITextProvider)!.Text;
        }
    }
}
