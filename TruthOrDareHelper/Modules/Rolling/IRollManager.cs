using Model;
using System.Collections.Generic;

namespace TruthOrDareHelper.Modules.Rolling
{
    public interface IRollManager
    {
        PlayerInfo? Reroll(ITruthOrDareSession session);

        List<PlayerPair> RollStandard(List<PlayerInfo> players, int maxParticipationStreak, int pairsToForm);
    }
}
