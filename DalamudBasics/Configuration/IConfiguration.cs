using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    }
}
