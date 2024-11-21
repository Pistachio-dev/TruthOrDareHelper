using DalamudBasics.Chat.Output;
using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface IToDChatOutput : IChatOutput
    {
        void ChatSoundWakeUp(PlayerInfo player);
        void TellWakeUp(PlayerInfo player);
        void WritePairs(List<PlayerPair> pairs);
    }
}
