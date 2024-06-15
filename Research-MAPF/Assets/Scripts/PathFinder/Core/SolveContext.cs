namespace PathFinder.Core
{
    public readonly struct SolveContext
    {
        public readonly int AgentIndex;
        public readonly int Start;
        public readonly int Goal;

        public SolveContext(int agentIndex, int start, int goal)
        {
            AgentIndex = agentIndex;
            Start = start;
            Goal = goal;
        }
    }
}