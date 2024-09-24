using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TruthOrDareHelper.DalamudWrappers
{
    public class TargetWrapper
    {
        public IGameObject? GetTarget()
        {
            return Plugin.TargetManager.Target;
        }

        public IPlayerCharacter? SearchTargetInObjectList(string targetNameWithoutWorld)
        {
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
