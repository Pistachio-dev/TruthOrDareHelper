using Dalamud.Game.ClientState.Objects.SubKinds;

namespace DalamudBasics.Targeting
{
    public interface ITargetingService
    {
        void ClearTarget();

        string GetTargetName();

        void RemovePlayerReference(string playerFullName);        

        bool TargetPlayer(string fullPlayerName);

        bool TrySaveTargetPlayerReference(out IPlayerCharacter? reference);
    }
}
