using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ConditionalDelegatePayload
    {
        public delegate bool OnChatMessageFunction(ChatChannelType chatChannel, DateTime timeStamp, string sender, string message); // Returns true if its mission is done and can be removed.

        public Guid Id { get; set; } = Guid.NewGuid();
        public List<string> MessageContentTrigger { get; set; }
        public bool IsMessageContentTriggerARegEx { get; set; }
        public string? PlayerNameTrigger { get; set; }
        public OnChatMessageFunction OnMessageWithValidTriggers { get; set; }

        // Leave strings null if they don't need to filter.
        public ConditionalDelegatePayload(string? contentTrigger, bool? isRegEx, string? playerNameTrigger, OnChatMessageFunction onTrigger)
        {
            if (contentTrigger != null)
            {
                MessageContentTrigger = new List<string> { contentTrigger };
            }

            IsMessageContentTriggerARegEx = isRegEx ?? false;
            PlayerNameTrigger = playerNameTrigger;
            OnMessageWithValidTriggers = onTrigger;
        }

        public ConditionalDelegatePayload(OnChatMessageFunction onTrigger, string playerNameTrigger, bool isRegEx, params string[] contentTrigger)
        {
            MessageContentTrigger = contentTrigger.ToList();
            IsMessageContentTriggerARegEx = isRegEx;
            PlayerNameTrigger = playerNameTrigger;
            OnMessageWithValidTriggers = onTrigger;
        }
    }
}
