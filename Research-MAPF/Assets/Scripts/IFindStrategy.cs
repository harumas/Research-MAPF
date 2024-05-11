using System.Collections.Generic;

namespace PathFinding
{
    public interface IFindStrategy
    {
        List<(Agent agent, List<int> path)> FindSolution(List<SearchContext> contexts);
    }
}