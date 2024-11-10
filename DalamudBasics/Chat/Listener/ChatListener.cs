using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Time;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DalamudBasics.Chat.Listener
{
    /// <summary>
    /// Provides an <see cref="OnChatMessage"/> event that will be raised on every game chat received.
    /// </summary>
    internal class ChatListener : IChatListener
    {
        private string pluginMessageMark;
        private readonly IChatGui chatGui;
        private readonly IClientState gameClient;
        private readonly ITimeUtils timeUtils;
        private readonly ILogService logService;

        public event IChatListener.ChatMessageHandler OnChatMessage;

        public ChatListener(IChatGui chatGui, IClientState gameClient, ITimeUtils timeUtils, ILogService logService)
        {
            this.chatGui = chatGui;
            this.gameClient = gameClient;
            this.timeUtils = timeUtils;
            this.logService = logService;
        }

        /// <summary>
        /// Initializes and attaches to the game the chat listener.
        /// </summary>
        /// <param name="pluginMessageMark"></param>
        public void InitializeAndRun(string pluginMessageMark)
        {
            this.pluginMessageMark = pluginMessageMark;
            AttachToGameChat();
        }

        private void AttachToGameChat()
        {
            chatGui.ChatMessage += PropagateToCustomEvent;
        }

        private void PropagateToCustomEvent(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            string messageAsString = message.ToString();
            if (messageAsString.Contains(pluginMessageMark, StringComparison.OrdinalIgnoreCase))
            {
                logService.Info($"Message sent by the plugin ignored: " + messageAsString);
                return;
            }

            string senderFullName = GetFullPlayerNameFromSenderData(sender);

            DateTime localTime = timeUtils.GetLocalDateTime();

            logService.Info($"Message processed and triggering custom event: " + messageAsString);
            OnChatMessage?.Invoke(type, senderFullName, messageAsString, localTime);
        }

        private string GetFullPlayerNameFromSenderData(SeString messageSender)
        {
            return messageSender.GetSenderFullName(gameClient);
        }
    }
}
