using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
using TruthOrDareHelper.DalamudWrappers.Interface;

namespace TruthOrDareHelper.DalamudWrappers
{
    public class TargetWrapper : ITargetWrapper
    {
        public IGameObject? GetTarget()
        {
            return Plugin.TargetManager.Target;
        }

        public IPlayerCharacter? SearchTargetInObjectList(string targetNameWithoutWorld)
        {
            // TODO: Separate this?
            return (IPlayerCharacter?)Plugin.ObjectTable.FirstOrDefault(o => o is IPlayerCharacter player && player.Name.ToString() == targetNameWithoutWorld);
        }

        public void Target(IPlayerCharacter reference)
        {
            Plugin.TargetManager.Target = reference;
        }

        public void ClearTarget()
        {
            Plugin.TargetManager.Target = null;
        }
    }
}
