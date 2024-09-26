using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using TruthOrDareHelper.DalamudWrappers;
using TruthOrDareHelper.DalamudWrappers.Interface;
using TruthOrDareHelper.Modules.Targeting.Interface;

namespace TruthOrDareHelper.Modules.Targeting
{
    public class TargetingHandler : ITargetingHandler
    {
        private readonly ILogWrapper log;
        private readonly ITargetWrapper targeting;
        private Dictionary<string, IPlayerCharacter> references = new();

        public TargetingHandler()
        {
            this.log = Plugin.Resolve<ILogWrapper>();
            this.targeting = Plugin.Resolve<ITargetWrapper>();
        }

        public string? AddReferenceToCurrentTarget()
        {
            var target = targeting.GetTarget();
            string targetFullName = GetTargetFullName(target);
            if (string.IsNullOrEmpty(targetFullName))
            {
                log.Error("Cannot save target player reference. Nothing is targeted, or it is not a player.");
                return null;
            }

            IPlayerCharacter targetedPlayer = (IPlayerCharacter)Plugin.TargetManager.Target!;
            references[targetFullName] = targetedPlayer;

            return targetFullName;
        }

        public bool TryRemoveTargetReference(string targetFullName)
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
            if (!references.TryGetValue(targetFullName, out var reference))
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
            return fullPlayerName == GetTargetFullName(targeting.GetTarget());
        }

        private string GetTargetFullName(IGameObject? target)
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
