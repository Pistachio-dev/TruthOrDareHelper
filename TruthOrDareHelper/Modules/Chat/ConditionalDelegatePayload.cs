using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ConditionalDelegatePayload
    {
        public delegate void OnChatMessageFunction(ChatChannelType chatChannel, DateTime timeStamp, string sender, string message);

        public Guid Id { get; set; } = Guid.NewGuid();
        public string? MessageContentTrigger { get; set; }
        public bool IsMessageContentTriggerARegEx { get; set; }
        public string? PlayerNameTrigger { get; set; }
        public OnChatMessageFunction OnMessageWithValidTriggers { get; set; }

        // Leave strings null if they don't need to filter.
        public ConditionalDelegatePayload(string? contentTrigger, bool? isRegEx, string? playerNameTrigger, OnChatMessageFunction onTrigger)
        {
            MessageContentTrigger = contentTrigger;
            IsMessageContentTriggerARegEx = isRegEx ?? false;
            PlayerNameTrigger = playerNameTrigger;
            OnMessageWithValidTriggers = onTrigger;
        }

    }
}
