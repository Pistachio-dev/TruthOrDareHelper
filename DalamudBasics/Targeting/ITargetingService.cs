namespace DalamudBasics.Targeting
{
    internal interface ITargetingService
    {
        void ClearTarget();

        void RemovePlayerReference(string playerFullName);

        bool SaveTargetPlayerReference();

        bool TargetPlayer(string fullPlayerName);
    }
}
