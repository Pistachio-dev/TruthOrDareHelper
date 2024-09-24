using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDareHelper.DalamudWrappers;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonRelicNoteBook;

namespace TruthOrDareHelper.Modules.Targeting
{
    public class TargetManager
    {
        private readonly LogWrapper log;
        private readonly TargetWrapper targeting;
        private Dictionary<string, IPlayerCharacter> references = new();

        public TargetManager(LogWrapper log, TargetWrapper targeting)
        {
            this.log = log;
            this.targeting = targeting;
        }

        public bool AddTargetReference()
        {
            var target = targeting.GetTarget();
            string targetName = GetTargetName(target);
            if (string.IsNullOrEmpty(targetName))
            {
                log.Error("Cannot save target player reference. Nothing is targeted, or it is not a player.");
                return false;
            }

            IPlayerCharacter targetedPlayer = (IPlayerCharacter)Plugin.TargetManager.Target!;
            references[targetName] = targetedPlayer;

            return true;
        }

        public bool RemoveTargetReference(string targetFullName)
        {
            if (references.ContainsKey(targetFullName))
            {
                references.Remove(targetFullName);
                return true;
            }

            return false;
        }

        public bool Target(string targetFullName)
        {
            if(!references.TryGetValue(targetFullName, out var reference))
            {
                log.Warning("Could not find the stored player reference. Trying to find if it is still spawned for you.");

                string name = targetFullName.Split("@")[0];
                reference = targeting.SearchTargetInObjectList(name);
                if (reference == null)
                {
                    log.Error($"Can't find {targetFullName} spawned anywhere.");
                    return false;
                }

                references[targetFullName] = reference;
            }

            targeting.Target(reference);

            return VerifyTargetingIntendedPlayer(targetFullName);
        }

        public void ClearTarget()
        {
            targeting.ClearTarget();
        }

        private bool VerifyTargetingIntendedPlayer(string fullPlayerName)
        {
            return fullPlayerName == GetTargetName(targeting.GetTarget());
        }

        private string GetTargetName(IGameObject? target)
        {
            if (target == null || target is not IPlayerCharacter pc || pc.HomeWorld.GameData == null)
                return string.Empty;

            return BuildFullPlayerName(pc);
        }

        private static string BuildFullPlayerName(IPlayerCharacter pc)
        {
            if (pc == null || pc.HomeWorld == null || pc.HomeWorld.GameData == null)
            {
                return string.Empty;
            }

            return $"{pc.Name}@{pc.HomeWorld.GameData.Name}";
        }
    }
}
