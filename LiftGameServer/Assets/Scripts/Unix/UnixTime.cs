using System;

namespace Lift
{
    public static class UnixTime
    {
        public static long Now
        {
            get
            {
                var raw = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
                if (raw > long.MaxValue || raw < long.MinValue)
                {
                    throw new Exception("time is broken.");
                }

                return (long)raw;
            }
        }
    }
}
