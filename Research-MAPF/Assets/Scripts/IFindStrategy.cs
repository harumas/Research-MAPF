using System.Collections.Generic;

namespace PathFinding
{
    public interface IFindStrategy
    {
        List<(Agent agent, List<int> path)> Solve(List<SearchContext> contexts);
    }
}