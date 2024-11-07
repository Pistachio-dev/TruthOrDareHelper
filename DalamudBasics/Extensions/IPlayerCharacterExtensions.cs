using Dalamud.Game.ClientState.Objects.SubKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Extensions
{
    public static class IPlayerCharacterExtensions
    {
        public static (string Name, string World) SplitFullName(this string fullName)
        {
            var split = fullName.Split('@');
            if (split.Length > 1)
            {
                return (split[0], split[1]);
            }
            return (split[0], string.Empty);
        }

        public static bool Matches(this IPlayerCharacter testedPlayer, string searchedName, string searchedWorld)
        {
            return testedPlayer.Name.ToString().Equals(searchedName, StringComparison.InvariantCultureIgnoreCase)
                && testedPlayer.GetWorldName().Equals(searchedWorld, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetWorldName(this IPlayerCharacter player)
        {
            return player?.HomeWorld?.GameData?.Name?.ToString() ?? string.Empty;
        }
    }
}