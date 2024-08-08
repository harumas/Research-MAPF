using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFinder.Core;
using UnityEngine;

namespace PathFinder.Solvers.CCBS
{
    public class ConstraindSIPP
    {
        private readonly Graph graph;
        private readonly List<Node> nodes;
        private readonly List<SafeInterval>[] intervals;

        public ConstraindSIPP(Graph graph, List<Node> nodes)
        {
            this.graph = graph;
            this.nodes = nodes;
            intervals = new List<SafeInterval>[graph.NodeCount];
        }

        public void ConstructInterval(List<List<int>> obstaclePath)
        {
            int maxTime = obstaclePath.Max(p => p.Count);
            BitArray[] conflictArray = new BitArray[maxTime];

            //時刻tにおける衝突情報を作る
            for (int t = 0; t < maxTime; t++)
            {
                conflictArray[t] = new BitArray(graph.NodeCount);

                foreach (List<int> path in obstaclePath)
                {
                    if (t < path.Count)
                    {
                        //時間tのパスのノードの場所を衝突したことにする
                        conflictArray[t][path[t]] = true;
                    }
                }
            }

            for (int i = 0; i < graph.NodeCount; i++)
            {
                intervals[i] = new List<SafeInterval>();

                int start = -1;
                for (int t = 0; t < maxTime; t++)
                {
                    bool isSafe = !conflictArray[t][i];

                    if (isSafe && start == -1)
                    {
                        start = t;
                    }

                    if (!isSafe && start != -1)
                    {
                        int end = t - 1;
                        intervals[i].Add(new SafeInterval(start, end));
                        start = -1;
                    }
                }

                if (start != -1)
                {
                    intervals[i].Add(new SafeInterval(start, maxTime - 1));
                }
            }
        }

        public List<Node> FindPath(
            int agentId,
            int start,
            int end,
            PriorityQueue<int, Constraint> constraints
        )
        {
            Debug.Log($"find path agent {agentId}");

            ResetNodes();
            var openList = new PriorityQueue<float, (float f, Node node)>(item => item.f, false);

            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            //初期ノードの作成
            startNode.Time = 0;
            startNode.H = Heuristic(startNode, targetNode);
            openList.Enqueue((0, startNode));

            // 待機し続けた時に打ち切る
            int timeout = graph.NodeCount;

            while (openList.Count > 0)
            {
                Node node = openList.Dequeue().node;

                //ゴールに到達したら
                if (node.Index == targetNode.Index)
                {
                    //ゴールした時間以上の制約を取得
                    var goalRange = constraints.Where(c => c.Time >= node.Time);
                    if (!Constraint.IsConstrained(agentId, node.Index, goalRange))
                    {
                        continue;
                    }

                    //親まで辿ってパスを返す
                    var r = RetracePath(node);

                    Debug.Log("found!");

                    StringBuilder builder = new StringBuilder();

                    builder.Append("{");
                    foreach (Node n in r)
                    {
                        builder.Append($"{n.Index:000}, ");
                    }

                    builder.Append("}");
                    Debug.Log(builder.ToString());

                    return r;
                }

                if (timeout < node.Time)
                {
                    continue;
                }

                List<int> nextNodes = graph.GetNextNodes(node.Index).ToList();
                nextNodes.Add(node.Index);

                foreach (int neighbourIndex in nextNodes)
                {
                    Node neighbour = nodes[neighbourIndex];

                    // 同じ時間の制約を取得
                    var range = constraints.Where(c => c.Time == node.Time + 1);

                    // 制約に引っかかったらスキップ
                    if (!Constraint.IsConstrained(agentId, node.Index, range))
                    {
                        continue;
                    }

                    Node next = neighbour.Clone();

                    next.H = Heuristic(neighbour, targetNode);
                    next.Time = node.Time + 1;
                    next.Parent = node;

                    openList.Enqueue((next.F, next));
                }
            }

            //パスを見つけられなかったらnull
            return null;
        }

        private float Heuristic(Node nodeA, Node nodeB)
        {
            float magnitude = (nodeA.Position - nodeB.Position).LengthSquared();
            return magnitude;
        }

        private List<Node> RetracePath(Node current)
        {
            var path = new List<Node>();

            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        private void ResetNodes()
        {
            foreach (Node node in nodes)
            {
                node.Reset();
            }
        }
    }
}