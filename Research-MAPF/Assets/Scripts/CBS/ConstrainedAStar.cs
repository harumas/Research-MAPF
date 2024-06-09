using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding.CBS
{
    public class ConstrainedAStar
    {
        private readonly Graph graph;
        private readonly List<Node> nodes;

        public ConstrainedAStar(Graph graph, GridGraphMediator mediator)
        {
            this.graph = graph;
            nodes = new List<Node>(graph.NodeCount);

            for (int i = 0; i < graph.NodeCount; i++)
            {
                nodes.Add(new Node(i, mediator.GetPos(i)));
            }
        }

        public List<Node> FindPath(
            int start,
            int end,
            List<Constraint> constraints
        )
        {
            Node startNode = nodes[start];
            Node targetNode = nodes[end];

            SortedSet<(float f, int node)> openSet = new SortedSet<(float f, int node)> { (0, start) };
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            Dictionary<int, int> gCosts = new Dictionary<int, int> { [start] = 0 };
            Dictionary<int, float> fCosts = new Dictionary<int, float> { [start] = Heuristic(startNode, targetNode) };

            while (openSet.Count > 0)
            {
                Node node = nodes[openSet.First().node];

                //ゴールに到達したら
                if (node == targetNode)
                {
                    //親まで辿ってパスを返す
                    return RetracePath(cameFrom, node.Index);
                }

                openSet.Remove(openSet.First());

                foreach (int neighbourIndex in graph.GetNextNodes(node.Index))
                {
                    Node neighbour = nodes[neighbourIndex];
                    int nextGCost = gCosts[node.Index] + 1;

                    //制約に引っかかったらスキップ
                    if (constraints.Exists(state => state.Node.Index == neighbour.Index && state.Time == nextGCost))
                    {
                        continue;
                    }

                    //ゴール方向に近づくノードだったら
                    if (!gCosts.ContainsKey(neighbourIndex) || nextGCost < gCosts[neighbourIndex])
                    {
                        cameFrom[neighbourIndex] = node.Index;
                        gCosts[neighbourIndex] = nextGCost;
                        fCosts[neighbourIndex] = nextGCost + Heuristic(neighbour, targetNode);

                        if (openSet.All(n => n.node != neighbour.Index))
                        {
                            openSet.Add((fCosts[neighbourIndex], neighbourIndex));
                        }
                    }

                    // 待機の選択肢を追加
                    int nextGCostWait = gCosts[node.Index] + 1;
                    if (constraints.Exists(state => state.Node.Index == node.Index && state.Time == nextGCostWait))
                    {
                        continue;
                    }

                    if (!gCosts.ContainsKey(node.Index) || nextGCostWait < gCosts[node.Index])
                    {
                        cameFrom[node.Index] = node.Index;
                        gCosts[node.Index] = nextGCostWait;
                        fCosts[node.Index] = nextGCostWait + Heuristic(node, nodes[end]);

                        if (openSet.All(n => n.node != node.Index))
                        {
                            openSet.Add((fCosts[node.Index], node.Index));
                        }
                    }
                }
            }

            List<Node> path = new List<Node>();
            path.Add(startNode);
            return path;
        }

        private float Heuristic(Node nodeA, Node nodeB)
        {
            float magnitude = Vector2.SqrMagnitude(nodeA.Position - nodeB.Position);
            return magnitude;
        }

        private List<Node> RetracePath(Dictionary<int, int> cameFrom, int current)
        {
            List<Node> path = new List<Node>() { nodes[current] };

            while (cameFrom.ContainsKey(current) && cameFrom[current] != current)
            {
                current = cameFrom[current];
                path.Add(nodes[current]);
            }

            path.Reverse();

            return path;
        }
    }
}