using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TruthOrDareHelper.DalamudWrappers.Interface;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ChatListener : IChatListener
    {
        public List<ConditionalDelegatePayload> triggers = new();

        private ILogWrapper log;
        private IChatWrapper chatRaw;

        public void AttachListener()
        {
            log = Plugin.Resolve<ILogWrapper>();
            chatRaw = Plugin.Resolve<IChatWrapper>();

            triggers.Add(new ConditionalDelegatePayload("asdf.*gg", true, "Pist", EchoMessage));
            chatRaw.AttachMethodToChatMessageReceived(OnChatMessage);
        }

        private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            string playerName = GetFullPlayerNameFromSenderData(sender);
            string messageString = message.ToString();
            ChatChannelType channelType = XivChatTypeToOwn(type);
            DateTime dateTime = DateTime.Now; // Let's just use local time for this.

            List<ConditionalDelegatePayload> triggersToRemove = new();
            foreach (var trigger in triggers)
            {
                if (DoesMessageTriggerPayload(playerName, messageString, trigger))
                {
                    log.Info($"Payload with ID {trigger.Id} triggered.");
                    bool success = trigger.OnMessageWithValidTriggers(channelType, dateTime, playerName, messageString);
                    if (success)
                    {
                        triggersToRemove.Add(trigger);
                    }
                }
            }

            triggers = triggers.Where(t => !triggersToRemove.Contains(t)).ToList();
        }

        private bool DoesMessageTriggerPayload(string sender, string message, ConditionalDelegatePayload payload)
        {
            bool triggered = true;
            if (payload.MessageContentTrigger != null)
            {
                foreach (var filter in payload.MessageContentTrigger)
                {
                    if (payload.IsMessageContentTriggerARegEx)
                    {
                        triggered &= new Regex(filter).IsMatch(message);
                    }
                    else
                    {
                        triggered &= message.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            if (payload.PlayerNameTrigger != null)
            {
                if (payload.PlayerNameTrigger.Contains("@"))
                {
                    log.Warning($"A player message trigger contains @, would should have been removed at this point, shouldn't it?");
                }

                triggered &= sender.Contains(payload.PlayerNameTrigger, StringComparison.InvariantCultureIgnoreCase);
            }

            return triggered;
        }

        public void AddConditionalDelegate(ConditionalDelegatePayload payload)
        {
            triggers.Add(payload);
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
                playerName = $"{Plugin.ClientState.LocalPlayer?.Name ?? "None"}@{Plugin.ClientState.LocalPlayer?.HomeWorld.GameData?.Name ?? "None"}";
            }

            return playerName;
        }

        private bool EchoMessage(ChatChannelType chatChannel, DateTime timeStamp, string sender, string message)
        {
            if (chatChannel == ChatChannelType.None)
            {
                return false;
            }
            chatRaw.Print($"[{timeStamp.ToShortDateString()}-{timeStamp.ToShortTimeString()}], by [{sender}] on channel [{chatChannel}]: {message}");

            return true;
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
