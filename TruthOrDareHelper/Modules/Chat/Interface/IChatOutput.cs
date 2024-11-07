using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface IChatOutput
    {
        void WriteChat(string message, ChatChannelType? chatChannel = null);

        void WritePairs(List<PlayerPair> pairs);
    }
}
