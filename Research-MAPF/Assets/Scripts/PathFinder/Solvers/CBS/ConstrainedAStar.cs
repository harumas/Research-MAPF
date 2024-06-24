using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
    /// <summary>
    /// 制約を考慮するA*アルゴリズム
    /// </summary>
    public class ConstrainedAStar
    {
        private readonly Graph graph;
        private readonly List<Node> nodes;

        public ConstrainedAStar(Graph graph, List<Node> nodes)
        {
            this.graph = graph;
            this.nodes = nodes;
        }

        public List<Node> FindPath(
            int start,
            int end,
            List<Constraint> constraints
        )
        {
            ResetNodes();
            var openList = new PriorityQueue<float, (float f, Node node)>(item => item.f, false);
            var closedList = new HashSet<Node>(new NodeComparer());

            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            //初期ノードの作成
            startNode.Time = 0;
            startNode.G = 0;
            startNode.H = Heuristic(startNode, targetNode);
            openList.Enqueue((0, startNode));

            while (openList.Count > 0)
            {
                Node node = openList.Dequeue().node;
                closedList.Add(node);

                //ゴールに到達したら
                if (node == targetNode)
                {
                    //親まで辿ってパスを返す
                    return RetracePath(node);
                }

                bool conflict = false;

                foreach (int neighbourIndex in graph.GetNextNodes(node.Index))
                {
                    Node neighbour = nodes[neighbourIndex];
                    int newG = node.G + 1;

                    // 制約に引っかかったらスキップ
                    int index = constraints.FindIndex(state =>
                        state.Node.Index == neighbour.Index && (state.Time == node.Time + 1 || state.Time == -1));
                    if (index != -1)
                    {
                        if (constraints[index].Time != -1)
                        {
                            conflict = true;
                        }

                        continue;
                    }

                    // 探索済みであればスキップ
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    if (openList.All(n => n.node.Index != neighbour.Index && n.node.Time != neighbour.Time))
                    {
                        neighbour.Parent = node;
                        neighbour.G = newG;
                        neighbour.H = Heuristic(neighbour, targetNode);
                        neighbour.Time = node.Time + 1;

                        openList.Enqueue((neighbour.F, neighbour));
                    }
                    else if (newG < neighbour.G)
                    {
                        neighbour.Parent = node;
                        neighbour.G = newG;
                    }
                }

                // 衝突したら待ちを考慮する
                if (conflict)
                {
                    int newG = node.G + 1;

                    Node newNode = node.Clone();
                    newNode.Parent = node;

                    newNode.Time = node.Time + 1;
                    newNode.G = newG;
                    newNode.H = Heuristic(node, targetNode);

                    if (openList.All(n => n.node.Index != newNode.Index && n.node.Time != newNode.Time))
                    {
                        openList.Enqueue((newNode.F, newNode));
                    }
                    else if (newG < node.G)
                    {
                        node.Parent = newNode;
                        node.G = newG;
                    }
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

    public class NodeComparer : IEqualityComparer<Node>
    {
        public bool Equals(Node a, Node b)
        {
            return b != null && a != null && a.Position == b.Position && a.Time == b.Time;
        }

        public int GetHashCode(Node obj)
        {
            return obj.Position.GetHashCode() ^ obj.Time.GetHashCode();
        }
    }
}