using Dalamud.Utility;
using SamplePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.Modules.Chat;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ChatOutput
    {
        private readonly Configuration configuration;
        private readonly ChatWrapper chat;
        private readonly LogWrapper log;

        private ChatChannelType DefaultChatOutput => configuration.DefaultChatChannel;

        public ChatOutput(Configuration configuration, ChatWrapper chat, LogWrapper log)
        {
            this.configuration = configuration;
            this.chat = chat;
            this.log = log;
        }

        public void WriteChat(string message, ChatChannelType? chatChannel = null)
        {
            if (message.IsNullOrEmpty())
            {
                return;
            }
            if (chatChannel == null)
            {
                chatChannel = DefaultChatOutput;
            }
            try
            {
                string messagePrefix = chatChannel switch
                {
                    ChatChannelType.Echo => "/echo ",
                    ChatChannelType.Party => "/p ",
                    ChatChannelType.Alliance => "/a ",
                    ChatChannelType.Say => "/s ",
                    ChatChannelType.Tell => "/tell ",
                    ChatChannelType.None => string.Empty,
                    _ => throw new Exception("Unknown chat channel.")
                };

                chat.SendMessage(messagePrefix, message);
            }
            catch
            {
                chat.PrintError("Invalid chat channel (or some odd error) for message: " + message + " with prefix " + chatChannel.ToString());
            }
        }
    }
}
