using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using System;
using System.Collections.Concurrent;

namespace DalamudBasics.Chat.Output
{
    internal class ChatOutput : IChatOutput
    {
        private ConcurrentQueue<ChatOutputQueuedMessage> chatQueue = new();
        private DateTime lastTimeChatWasWritten = DateTime.MinValue;
        private readonly IConfiguration configuration;
        private readonly ILogService logService;
        private readonly IClientChatGui chatGui;

        private XivChatType DefaultOutputChatType
        {
            get
            {
                return configuration.DefaultOutputChatType;
            }
        }

        public ChatOutput(IConfiguration configuration, ILogService logService, IClientChatGui chatGui)
        {
            this.configuration = configuration;
            this.logService = logService;
            this.chatGui = chatGui;
        }


        public void WriteChat(string message, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0)
        {
            chatQueue.Enqueue(new ChatOutputQueuedMessage(message, chatChannel, minSpacingBeforeInMs));
        }

        public void SendTell(string message, string playerFullName, string playerHomeWorld, XivChatType? chatChannel = null, int minSpacingBeforeInMs = 0)
        {
            string messageWithRecipient = $"{playerFullName}@{playerHomeWorld} {message}";
            WriteChat(messageWithRecipient, chatChannel, minSpacingBeforeInMs);
        }

        public void AttachToGameLogicLoop(IFramework framework)
        {
            framework.Update += Tick;
        }

        private void Tick(IFramework framework)
        {
            while (chatQueue.TryPeek(out ChatOutputQueuedMessage? nextChatPayload))
            {
                if ((DateTime.Now - lastTimeChatWasWritten).TotalMilliseconds < nextChatPayload.SpacingBeforeInMs)
                {
                    break;
                }

                if (chatQueue.TryDequeue(out nextChatPayload!))
                {
                    ActuallyWriteChat(nextChatPayload);
                    lastTimeChatWasWritten = DateTime.Now;
                }
            }
        }

        private void ActuallyWriteChat(ChatOutputQueuedMessage payload)
        {

            if (payload.Message.IsNullOrEmpty())
            {
                return;
            }
            if (payload.ChatChannel == null)
            {
                payload.ChatChannel = DefaultOutputChatType;
            }
            try
            {
                var messagePrefix = payload.ChatChannel switch
                {
                    XivChatType.Echo => "/echo",
                    XivChatType.Party => "/p",
                    XivChatType.Alliance => "/a",
                    XivChatType.Say => "/s",
                    XivChatType.TellOutgoing => "/tell",
                    XivChatType.Yell => "/yell",
                    XivChatType.Shout => "/shout",
                    XivChatType.CrossLinkShell1 => "/cwlinkshell1",
                    XivChatType.CrossLinkShell2 => "/cwlinkshell2",
                    XivChatType.CrossLinkShell3 => "/cwlinkshell3",
                    XivChatType.CrossLinkShell4 => "/cwlinkshell4",
                    XivChatType.CrossLinkShell5 => "/cwlinkshell5",
                    XivChatType.CrossLinkShell6 => "/cwlinkshell6",
                    XivChatType.CrossLinkShell7 => "/cwlinkshell7",
                    XivChatType.CrossLinkShell8 => "/cwlinkshell8",
                    XivChatType.Ls1 => "/linkshell1",
                    XivChatType.Ls2 => "/linkshell2",
                    XivChatType.Ls3 => "/linkshell3",
                    XivChatType.Ls4 => "/linkshell4",
                    XivChatType.Ls5 => "/linkshell5",
                    XivChatType.Ls6 => "/linkshell6",
                    XivChatType.Ls7 => "/linkshell7",
                    XivChatType.Ls8 => "/linkshell8",

                    XivChatType.None => string.Empty, // I use this to trigger emotes. The actual XivChatType.Emote is the displayed message in /emotelog
                    _ => throw new Exception("Unsupported output chat channel.")
                };

                var sanitizedText = ECommons.Automation.Chat.Instance.SanitiseText(payload.Message);
                string fullChatString = $"{messagePrefix} {sanitizedText}";
                ECommons.Automation.Chat.Instance.SendMessage(fullChatString);
                if (configuration.LogOutgoingChatOutput)
                {
                    logService.Info("[Chat]" + fullChatString);
                }
            }
            catch
            {
                chatGui.PrintError("Invalid chat channel (or some odd error) for message: " + payload.Message + " with prefix " + payload.ChatChannel.ToString());
            }
        }
    }
}
