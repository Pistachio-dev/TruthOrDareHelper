using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.ChatMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Dalamud.Plugin.Services.IChatGui;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ChatListener
    {       
        public void AttachListener()
        {
            Plugin.Chat.ChatMessage += OnChatMessage;
        }

        private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            string playerName = GetFullPlayerNameFromSenderData(sender);

            ChatChannelType channelType = XivChatTypeToOwn(type);
            DateTime dateTime = DateTime.Now; // Let's just use local time for this.

            EchoMessage(channelType, dateTime, playerName, message.ToString());
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
                playerName = $"{Plugin.ClientState.LocalPlayer?.Name ?? "None"}@{Plugin.ClientState.LocalPlayer?.HomeWorld.GetWithLanguage(Dalamud.Game.ClientLanguage.English)?.Name ?? "None"}";
            }

            return playerName;
        }
        private void EchoMessage(ChatChannelType chatChannel, DateTime timeStamp, string sender, string message)
        {
            if (chatChannel == ChatChannelType.None)
            {
                return;
            }
            Plugin.Chat.Print($"[{timeStamp.ToShortDateString()}-{timeStamp.ToShortTimeString()}], by [{sender}] on channel [{chatChannel}]: {message}");
        }

        // We don't need to handle all types for this.
        private ChatChannelType XivChatTypeToOwn(XivChatType chatType)
        {
            return chatType switch
            {
                XivChatType.Echo => ChatChannelType.Echo,
                XivChatType.Say => ChatChannelType.Say,
                XivChatType.Party => ChatChannelType.Party,
                XivChatType.Alliance => ChatChannelType.Alliance,
                XivChatType.TellIncoming => ChatChannelType.Tell,
                _ => ChatChannelType.None,
            };
        }

        
    }
}
