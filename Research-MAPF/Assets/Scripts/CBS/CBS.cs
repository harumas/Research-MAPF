using System.Collections.Generic;
using UnityEngine;

namespace PathFinding.CBS
{
    public class CBS : IFindStrategy
    {
        private readonly ConstrainedAStar pathFinder;

        public CBS(Graph graph, GridGraphMediator mediator)
        {
            pathFinder = new ConstrainedAStar(graph, mediator);
        }

        public List<(Agent agent, List<int> path)> FindSolution(List<SearchContext> contexts)
        {
            
        }
    }
}