using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerReferenceMap = System.Collections.Generic.Dictionary<string, Dalamud.Game.ClientState.Objects.SubKinds.IPlayerCharacter>;

namespace DalamudBasics.Targeting
{
    internal class TargetingService : ITargetingService
    {
        private readonly ITargetManager dalamudTargetManager;
        private readonly IObjectTable gameObjectTable;
        private readonly ILogService logService;
        private readonly IClientChatGui chatGui;
        private PlayerReferenceMap playerRefs;

        public TargetingService(ITargetManager targetManager, IObjectTable gameObjectTable, ILogService logService, IClientChatGui chatGui)
        {
            playerRefs = [];
            this.dalamudTargetManager = targetManager;
            this.gameObjectTable = gameObjectTable;
            this.logService = logService;
            this.chatGui = chatGui;
        }

        public void RemovePlayerReference(string playerFullName)
        {
            playerRefs.Remove(playerFullName);
        }

        public bool SaveTargetPlayerReference()
        {
            var targetName = GetTargetName();
            if (string.IsNullOrEmpty(targetName))
            {
                chatGui.PrintError("Cannot save target player reference. Nothing is targeted, or it is not a player.");
                logService.Info("Failed attemp to save targeted reference: not a player");

                return false;
            }

            // At this point we know it is a player character, valid, not null.
            var targetedPlayer = (IPlayerCharacter)dalamudTargetManager.Target!;
            playerRefs[targetName] = targetedPlayer;
            logService.Debug("Stored reference for player " + targetName);

            return true;
        }

        public void ClearTarget()
        {
            dalamudTargetManager.Target = null;
        }

        public bool TargetPlayer(string fullPlayerName)
        {
            var playerCharacter = playerRefs.GetValueOrDefault(fullPlayerName);
            if (playerCharacter == null)
            {
                chatGui.Print("Could not find the stored player reference. Trying to find if it is still spawned for you.");

                (string name, string world) = fullPlayerName.SplitFullName();
                playerCharacter = GetPlayerReferenceFromObjectTable(name, world);
                if (playerCharacter == null)
                {
                    chatGui.PrintError($"Can't find {fullPlayerName} spawned anywhere.");
                    logService.Info($"Failed to find player in object table: " + fullPlayerName);

                    return false;
                }

                playerRefs[BuildFullPlayerName(playerCharacter)] = playerCharacter;
                chatGui.Print($"{fullPlayerName} found, recreating reference.");
                logService.Info($"Failed to find player in object table: " + fullPlayerName);
            }

            dalamudTargetManager.Target = playerCharacter;
            logService.Info($"Targeting player: " + fullPlayerName);

            return VerifyTargeting(fullPlayerName);
        }

        private IPlayerCharacter? GetPlayerReferenceFromObjectTable(string name, string world)
        {
            return (IPlayerCharacter?)gameObjectTable.FirstOrDefault(o => o is IPlayerCharacter player && player.Matches(name, world));
        }

        private string GetTargetName()
        {
            var target = dalamudTargetManager.Target;
            if (target == null || target is not IPlayerCharacter pc || pc.HomeWorld.GameData == null)
                return string.Empty;

            return BuildFullPlayerName(pc);
        }

        private static string BuildFullPlayerName(IPlayerCharacter pc)
        {
            if (pc?.HomeWorld?.GameData == null)
            {
                return string.Empty;
            }

            return $"{pc.Name}@{pc.HomeWorld.GameData.Name}";
        }

        private bool VerifyTargeting(string fullPlayerName)
        {
            return fullPlayerName == GetTargetName();
        }
    }
}
