using DalamudBasics.Chat.Output;
using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface IToDChatOutput : IChatOutput
    {
        void WritePairs(List<PlayerPair> pairs);
    }
}
