using System.Collections.Generic;
using System.Linq;

namespace PathFinding.CBS
{
    public class ConstraintNode
    {
        public readonly List<Constraint>[] Constraints;
        public readonly List<List<Node>> Solution = new List<List<Node>>();
        public int Cost;

        public ConstraintNode(List<Constraint>[] constraints, List<List<Node>> solution, int cost)
        {
            Constraints = new List<Constraint>[constraints.Length];
            for (int i = 0; i < Constraints.Length; i++)
            {
                Constraints[i] = new List<Constraint>(constraints[i]);
            }

            foreach (List<Node> path in solution)
            {
                Solution.Add(new List<Node>(path));
            }

            Cost = cost;
        }

        public int GetConstraintsCount()
        {
            return Constraints.Sum(t => t.Count);
        }
    }
}