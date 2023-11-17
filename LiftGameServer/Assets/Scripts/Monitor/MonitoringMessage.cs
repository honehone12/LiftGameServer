namespace Lift
{
    [System.Serializable]
    public class MonitoringMessage
    {
        public byte[] GuidRaw;
        // total clients connected including that has no right session
        public long ConnectionCount;
        // total sessions including disconnected clients.
        public long SessionCount;
        // clinets that have right session. 
        public long ActiveSessionCount;

        public MonitoringMessage(
            byte[] guidRaw, 
            long connectionCount, 
            long sessionCount, 
            long activeSessionCount)
        {
            GuidRaw = guidRaw;
            ConnectionCount = connectionCount;
            SessionCount = sessionCount;
            ActiveSessionCount = activeSessionCount;
        }
    }
}
