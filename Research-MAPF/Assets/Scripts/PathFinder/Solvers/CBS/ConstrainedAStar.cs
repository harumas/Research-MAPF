using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Solvers.CBS
{
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
            foreach (Node node in nodes)
            {
                node.Reset();
            }

            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            startNode.Time = 0;
            startNode.G = 0;
            startNode.H = Heuristic(startNode, targetNode);

            PriorityQueue<float, (float f, Node node)> openList = new PriorityQueue<float, (float f, Node node)>(item => item.f, false);
            openList.Enqueue((0, startNode));

            HashSet<Node> closedList = new HashSet<Node>(new NodeComparer());

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
                int newTime = node.Time + 1;
                int newG = node.G + 1;

                foreach (int neighbourIndex in graph.GetNextNodes(node.Index))
                {
                    Node neighbour = nodes[neighbourIndex];

                    //制約に引っかかったらスキップ
                    int index = constraints.FindIndex(state => state.Node.Index == neighbour.Index && (state.Time == newTime || state.Time == -1));
                    if (index != -1)
                    {
                        if (constraints[index].Time != -1)
                        {
                            conflict = true;
                        }

                        continue;
                    }

                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    if (openList.All(n => n.node.Index != neighbour.Index))
                    {
                        neighbour.Parent = node;
                        neighbour.G = newG;
                        neighbour.H = Heuristic(neighbour, targetNode);
                        openList.Enqueue((neighbour.F, neighbour));
                    }
                    else if (newG < neighbour.G)
                    {
                        neighbour.Parent = node;
                        neighbour.G = newG;
                    }
                }

                if (conflict)
                {
                    Node newNode = node.Clone();
                    newNode.Time = newTime;
                    newNode.G = newG;
                    newNode.H = Heuristic(node, targetNode);
                    newNode.Parent = node;

                    if (openList.All(n => n.node.Index != node.Index))
                    {
                        openList.Enqueue((node.F, node));
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