using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFinder.Core;
using UnityEngine;
using Wanna.DebugEx;

namespace PathFinder.Solvers.CBS
{
    /// <summary>
    /// Conflict Based Search Algorithm
    /// </summary>
    public class CBS : ISolver
    {
        private readonly ConstrainedAStar pathFinder;
        private readonly ConflictFinder conflictFinder;
        private const int maxSolveCount = 512;

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
            PriorityQueue<int, Constraint> constraints = new PriorityQueue<int, Constraint>(item => item.Time, false);
            List<List<Node>> solution = GetSolution(contexts, constraints);
            int cost = solution.Sum(path => path.Count);
            ConstraintNode node = new ConstraintNode(constraints, solution, cost);

            openList.Add(node);

            int solveCount = 0;

            // maxSolveCountを超えたら探索終了
            while (openList.Count > 0 && solveCount <= maxSolveCount)
            {
                solveCount++;

                node = GetMinCostNode(openList);
                openList.Remove(node);

                Conflict conflict = conflictFinder.GetConflicts(node.Solution);

                //衝突がなかったら終了
                if (conflict == null)
                {
                    //探索結果が0 or 前の結果より小さいコストのSolutionが見つかった
                    if (resultNode == null || node.Cost < resultNode.Cost)
                    {
                        resultNode = node;
                    }

                    continue;
                }

                foreach (int agentID in conflict.Agents)
                {
                    // 前の制約のコピー
                    PriorityQueue<int, Constraint> newConstraints = constraints.Clone();

                    // 制約の追加
                    newConstraints.Enqueue(new Constraint(agentID, conflict.Node, conflict.Time));

                    StringBuilder bu = new StringBuilder();

                    foreach (Constraint co in newConstraints)
                    {
                        bu.Append($"{{{agentID}, {co.Node.Index}, {co.Time}}}\n");
                    }

                    Debug.Log(bu.ToString());

                    // 新しい制約を元に解決
                    List<List<Node>> newSolution = GetSolution(contexts, newConstraints);

                    // 解決できなかった場合はスキップ
                    if (newSolution.Any(sol => sol == null))
                    {
                        Debug.Log("end");
                        continue;
                    }

                    StringBuilder builder = new StringBuilder();

                    foreach (List<Node> nodes in newSolution)
                    {
                        builder.Append("{");
                        foreach (Node n in nodes)
                        {
                            builder.Append($"{n.Index:000}, ");
                        }

                        builder.Append("}");
                        Debug.Log(builder.ToString());
                        builder.Clear();
                    }

                    int newCost = solution.Sum(path => path.Count);

                    // 解決したノードを追加
                    ConstraintNode newNode = new ConstraintNode(newConstraints, newSolution, newCost);
                    openList.Add(newNode);
                }
            }

            // 解決できなかった
            if (resultNode == null)
            {
                return null;
            }

            //エージェントとパス情報の作成
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
            PriorityQueue<int, Constraint> constraints
        )
        {
            List<List<Node>> solution = new List<List<Node>>();

            foreach (SolveContext context in contexts)
            {
                List<Node> path = pathFinder.FindPath(context.AgentIndex, context.Start, context.Goal, constraints);
                solution.Add(path);
            }

            return solution;
        }
    }
}