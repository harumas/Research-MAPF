using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    public class ConstraintNode
    {
        public readonly PriorityQueue<int, Constraint> Constraints;
        public readonly List<List<Node>> Solution = new List<List<Node>>();
        public readonly int Cost;

        public ConstraintNode(PriorityQueue<int, Constraint> constraints, List<List<Node>> solution, int cost)
        {
            Constraints = constraints.Clone();

            foreach (List<Node> path in solution)
            {
                Solution.Add(new List<Node>(path));
            }

            Cost = cost;
        }

        public int GetConstraintsCount()
        {
            return Constraints.Count;
        }
    }
}