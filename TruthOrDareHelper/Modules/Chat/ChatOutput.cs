using Dalamud.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.DalamudWrappers.Interface;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ChatOutput : IChatOutput
    {
        private readonly IConfiguration configuration;
        private readonly IChatWrapper chat;
        private readonly ILogWrapper log;

        private ChatChannelType DefaultChatOutput => configuration.DefaultChatChannel;

        public ChatOutput()
        {
            configuration = Plugin.Resolve<IConfiguration>(); ;
            chat = Plugin.Resolve<IChatWrapper>();
            log = Plugin.Resolve<ILogWrapper>();
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
