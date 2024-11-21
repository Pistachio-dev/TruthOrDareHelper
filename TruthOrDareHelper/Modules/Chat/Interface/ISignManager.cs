using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Chat.Interface
{
    public interface ISignManager
    {
        void ApplyMarks(List<PlayerPair> playerPairsToMark);

        void ClearMarks(List<PlayerPair> markedPlayerPairs);

        void MarkPlayer(PlayerInfo player, bool isWinner);

        void UnmarkPlayer(PlayerInfo player);
    }
}
