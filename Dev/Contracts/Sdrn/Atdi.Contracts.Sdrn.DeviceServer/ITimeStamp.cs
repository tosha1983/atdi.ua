namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITimeStamp
    {
        long Milliseconds { get; }

        long Value { get; }

        long Ticks { get; }

        bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds);

        bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds, out long lateness);
    }
}