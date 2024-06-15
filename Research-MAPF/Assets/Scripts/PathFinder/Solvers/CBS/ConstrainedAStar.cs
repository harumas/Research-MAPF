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
            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            PriorityQueue<float, (float f, int node)> openList = new PriorityQueue<float, (float f, int node)>(item => item.f, false);
            openList.Enqueue((0, start));

            SortedDictionary<int, int> cameFrom = new SortedDictionary<int, int>();
            Dictionary<int, int> gCosts = new Dictionary<int, int> { [start] = 0 };
            Dictionary<int, float> fCosts = new Dictionary<int, float> { [start] = Heuristic(startNode, targetNode) };

            while (openList.Count > 0)
            {
                Node node = nodes[openList.Dequeue().node];

                //ゴールに到達したら
                if (node == targetNode)
                {
                    //親まで辿ってパスを返す
                    return RetracePath(cameFrom, node.Index);
                }

                foreach (int neighbourIndex in graph.GetNextNodes(node.Index))
                {
                    Node neighbour = nodes[neighbourIndex];
                    int nextGCost = gCosts[node.Index] + 1;

                    //制約に引っかかったらスキップ
                    int index = constraints.FindIndex(state => state.Node.Index == neighbour.Index && (state.Time == nextGCost || state.Time == -1));
                    if (index != -1)
                    {
                        if (constraints[index].Time != -1)
                        {
                            cameFrom[nextGCost] = node.Index;
                            gCosts[node.Index] += 1;
                            fCosts[node.Index] += 1;

                            if (openList.All(n => n.node != node.Index))
                            {
                                openList.Enqueue((fCosts[node.Index], node.Index));
                            }
                        }

                        continue;
                    }

                    //ゴール方向に近づくノードだったら
                    if (!gCosts.ContainsKey(neighbourIndex) || nextGCost < gCosts[neighbourIndex])
                    {
                        cameFrom[nextGCost] = node.Index;
                        gCosts[neighbourIndex] = nextGCost;
                        fCosts[neighbourIndex] = nextGCost + Heuristic(neighbour, targetNode);

                        if (openList.All(n => n.node != neighbour.Index))
                        {
                            openList.Enqueue((fCosts[neighbourIndex], neighbourIndex));
                        }
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

        private List<Node> RetracePath(SortedDictionary<int, int> cameFrom, int current)
        {
            List<Node> path = new List<Node> { };

            int goal = current;

            foreach (int node in cameFrom.Select(item => item.Value))
            {
                current = node;
                path.Add(nodes[current]);
            }

            path.Add(nodes[goal]);

            return path;
        }
    }
}