namespace PathFinder.Solvers.CCBS
{
    public readonly struct Constraint
    {
        public readonly int AgentId;
        public readonly int From;
        public readonly int To;
        public readonly float StartTime;
        public readonly float EndTime;

        public Constraint(int agentId, int from, int to, float startTime, float endTime)
        {
            AgentId = agentId;
            From = from;
            To = to;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}