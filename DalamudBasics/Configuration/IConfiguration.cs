using Dalamud.Game.Text;
using System.Runtime.Serialization;

namespace DalamudBasics.Configuration
{
    public interface IConfiguration : ISerializable
    {
        // If your plugin does not write to chat, set this to None
        public XivChatType DefaultOutputChatType { get; set; }

        // If true, all chat meant to be sent to the server will be logged.
        public bool LogOutgoingChatOutput { get; set; }

        // If true, all chat written only visually and not sent will be logged.
        public bool LogClientOnlyChatOutput { get; set; }

        public int SayDelayInMs { get; set; }
    }
}
