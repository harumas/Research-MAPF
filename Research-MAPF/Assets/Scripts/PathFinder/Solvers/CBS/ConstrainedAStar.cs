using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;
using Constraint = PathFinder.Solvers.CBS.Constraint;

namespace PathFinder.Solvers.CBS
{
    /// <summary>
    /// 制約を考慮するA*アルゴリズム
    /// </summary>
    public class ConstrainedAStar
    {
        private readonly Graph graph;
        private readonly List<Node> nodes;
        private readonly int timeout;

        public ConstrainedAStar(Graph graph, List<Node> nodes)
        {
            this.graph = graph;
            this.nodes = nodes;

            timeout = 10;
        }

        public List<Node> FindPath(
            int agentId,
            int start,
            int end,
            PriorityQueue<int, Constraint> constraints
        )
        {
            ResetNodes();
            var openList = new PriorityQueue<float, (float f, Node node)>(item => item.f, false);

            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            //初期ノードの作成
            startNode.Time = 0;
            startNode.H = Heuristic(startNode, targetNode);
            openList.Enqueue((0, startNode));

            while (openList.Count > 0)
            {
                Node node = openList.Dequeue().node;

                //ゴールに到達したら
                if (node.Index == targetNode.Index)
                {
                    var range = constraints.Where(c => c.Time >= node.Time);
                    if (!Constraint::IsConstrained(agentId, node.Index, range))
                    {
                        continue;
                    }

                    //親まで辿ってパスを返す
                    return RetracePath(node);
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
                    if (!Constraint::IsConstrained(agentId, node.Index, range))
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