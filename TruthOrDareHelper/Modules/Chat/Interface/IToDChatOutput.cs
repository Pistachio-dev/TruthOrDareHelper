using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Chat.Interface
{
    internal interface IToDChatOutput
    {
        void WritePairs(List<PlayerPair> pairs);
    }
}