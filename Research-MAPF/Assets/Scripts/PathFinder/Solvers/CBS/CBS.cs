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

        public CBS(Graph graph, List<Node> nodes)
        {
            pathFinder = new ConstrainedAStar(graph, nodes);
            conflictFinder = new ConflictFinder();
        }

        public List<(int agentIndex, List<int> path)> Solve(List<SolveContext> contexts)
        {
            ConstraintNode resultNode = null;

            //CTのルートノードを作成
            PriorityQueue<int, Constraint> constraints = new PriorityQueue<int, Constraint>(item => item.Time, false);
            List<List<Node>> solution = GetSolution(contexts, constraints);
            int cost = solution.Sum(path => path.Count);
            ConstraintNode node = new ConstraintNode(constraints, solution, cost);

            //オープンリストにルートを追加
            PriorityQueue<int, ConstraintNode> q = new PriorityQueue<int, ConstraintNode>(item => item.Cost, false);
            q.Enqueue(node);

            while (q.Count > 0)
            {
                node = q.Dequeue();

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

                Debug.Log(conflict.ToString());

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

                    int newCost = solution.Sum(path => path.Count);

                    // 解決したノードを追加
                    ConstraintNode newNode = new ConstraintNode(newConstraints, newSolution, newCost);
                    q.Enqueue(newNode);
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