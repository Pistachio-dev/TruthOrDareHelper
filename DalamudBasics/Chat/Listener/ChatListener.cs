using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public delegate void ChatMessageHandler(XivChatType type, string senderFullName, string message, DateTime receivedAt);

        public event ChatMessageHandler OnChatMessage;

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
            Payload? playerPayload = messageSender.Payloads.FirstOrDefault(p => p.Type == PayloadType.Player);
            string playerName = "Uninitialized";
            if (playerPayload != null)
            {
                var groups = new Regex("Player - PlayerName: ([^\\s,]+ [^\\s,]+), ServerId.*ServerName: (\\S+)").Match(playerPayload.ToString()!).Groups;
                if (groups.Count < 3)
                {
                    playerName = "Could not be captured";
                }
                else
                {
                    playerName = $"{groups[1]}@{groups[2]}";
                }
            }
            else
            {
                // I'm going to assume this is always the mod runner, since it only applies to me when testing.
                playerName = $"{gameClient.LocalPlayer?.GetFullName() ?? "Nobody"}";
            }

            return playerName;
        }
    }
}
