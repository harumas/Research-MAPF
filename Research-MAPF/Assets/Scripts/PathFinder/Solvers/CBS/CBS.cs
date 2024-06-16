using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    public class CBS : ISolver
    {
        private readonly ConstrainedAStar pathFinder;
        private readonly ConflictFinder conflictFinder;
        private const int MaxSolveCount = 2048;

        public CBS(Graph graph, List<Node> nodes)
        {
            pathFinder = new ConstrainedAStar(graph, nodes);
            conflictFinder = new ConflictFinder();
        }

        public List<(int agentIndex, List<int> path)> Solve(List<SolveContext> contexts)
        {
            int agentCount = contexts.Count;
            List<ConstraintNode> openList = new List<ConstraintNode>();
            ConstraintNode resultNode = null;

            //CTのルートノードを作成
            List<Constraint>[] emptyConstraints = GetEmptyConstraints(agentCount);
            List<List<Node>> solution = GetSolution(contexts, emptyConstraints);
            int cost = solution.Sum(path => path.Count);
            ConstraintNode node = new ConstraintNode(emptyConstraints, solution, cost);

            openList.Add(node);

            int solveCount = 0;

            while (openList.Count > 0 && solveCount <= MaxSolveCount)
            {
                solveCount++;

                node = GetMinCostNode(openList);
                openList.Remove(node);

                List<Conflict> conflicts = conflictFinder.GetConflicts(node.Solution);

                //衝突がなかったら終了
                if (conflicts.Count == 0)
                {
                    if (resultNode == null || node.Cost < resultNode.Cost)
                    {
                        resultNode = node;
                    }

                    continue;
                }

                foreach (Conflict conflict in conflicts)
                {
                    foreach (int agentID in conflict.Agents)
                    {
                        // copy constraints
                        List<Constraint>[] newConstraints = new List<Constraint>[agentCount];
                        for (int i = 0; i < newConstraints.Length; i++)
                        {
                            newConstraints[i] = new List<Constraint>(node.Constraints[i]);
                        }

                        // add new constraint
                        newConstraints[agentID].Add(new Constraint(conflict.Node, conflict.Time));

                        // solve with new constraints
                        List<List<Node>> newSolution = GetSolution(contexts, newConstraints, agentID);

                        //解決できなかった場合はスキップ
                        if (newSolution.Any(sol => sol == null))
                        {
                            continue;
                        }

                        int newCost = solution.Sum(path => path.Count);

                        // add new node
                        ConstraintNode newNode = new ConstraintNode(newConstraints, newSolution, newCost);
                        openList.Add(newNode);
                    }
                }
            }

            //解決できなかった
            if (resultNode == null)
            {
                return null;
            }

            List<(int agentIndex, List<int> path)> results = new List<(int agentIndex, List<int> path)>(contexts.Count);

            for (int i = 0; i < contexts.Count; i++)
            {
                List<Node> path = resultNode.Solution[i];
                results.Add((contexts[i].AgentIndex, path.Select(item => item.Index).ToList()));
            }

            return results;
        }

        private static ConstraintNode GetMinCostNode(List<ConstraintNode> nodes)
        {
            ConstraintNode node = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Cost < node.Cost)
                {
                    node = nodes[i];
                }
                else if (nodes[i].Cost == node.Cost && nodes[i].GetConstraintsCount() < node.GetConstraintsCount())
                {
                    node = nodes[i];
                }
            }

            return node;
        }

        private List<List<Node>> GetSolution(
            List<SolveContext> contexts,
            List<Constraint>[] constraints,
            int currentAgent = -1)
        {
            List<List<Node>> solution = new List<List<Node>>();
            List<Node> constrainedPath = new List<Node>();

            // solve constraint agent first
            if (currentAgent != -1)
            {
                SolveContext context = contexts[currentAgent];
                constrainedPath = pathFinder.FindPath(context.Start, context.Goal, constraints[currentAgent]);
            }

            // solve the rest of the agents
            for (int i = 0; i < constraints.Length; i++)
            {
                if (currentAgent == -1 || i != currentAgent)
                {
                    SolveContext context = contexts[i];
                    List<Node> path = pathFinder.FindPath(context.Start, context.Goal, constraints[i]);
                    solution.Add(path);
                }

                if (currentAgent != -1 && i == currentAgent)
                {
                    solution.Add(constrainedPath);
                }
            }

            return solution;
        }

        private List<Constraint>[] GetEmptyConstraints(int count)
        {
            List<Constraint>[] constraints = new List<Constraint>[count];

            for (int i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new List<Constraint>();
            }

            return constraints;
        }
    }
}