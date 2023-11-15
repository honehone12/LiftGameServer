namespace Lift
{
    [System.Serializable]
    public struct MonitoringMessage
    {
        public uint ConnectionCount;
        public uint SessionCount;
        public uint ActiveSessionCount;

        public MonitoringMessage(uint connectionCount, uint sessionCount, uint activeSessionCount)
        {
            ConnectionCount = connectionCount;
            SessionCount = sessionCount;
            ActiveSessionCount = activeSessionCount;
        }
    }
}
