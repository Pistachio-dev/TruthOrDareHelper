namespace DalamudBasics.Time
{
    public interface ITimeUtils
    {
        long GetNowUnixTimestamp(bool useUtc = true);
    }
}
