using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace TruthOrDareHelper.DalamudWrappers.Interface
{
    public interface ITargetWrapper
    {
        void ClearTarget();
        IGameObject? GetTarget();
        IPlayerCharacter? SearchTargetInObjectList(string targetNameWithoutWorld);
        void Target(IPlayerCharacter reference);
    }
}
