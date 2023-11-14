namespace Lift
{
    public struct Session
    {
        public static Session NewActiveSession(ulong connectionId)
        {
            return new Session(connectionId, true, UnixTime.Now, 0, 0);
        }

        public ulong connectionId;
        public bool isActive;
        public long timeConnected;
        public long timeReconnected;
        public long timeDisconnected;

        public Session(
            ulong connectionId, 
            bool isActive, 
            long timeConnected,
            long timeReconnected,
            long timeDisconnected)
        {
            this.connectionId = connectionId;
            this.isActive = isActive;
            this.timeConnected = timeConnected;
            this.timeReconnected = timeReconnected;
            this.timeDisconnected = timeDisconnected;
        }
    }
}
