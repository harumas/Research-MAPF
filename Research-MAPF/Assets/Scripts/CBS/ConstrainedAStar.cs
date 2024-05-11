using System;
using UnityEngine;
using System.Collections.Generic;

namespace PathFinding.CBS
{
    public class ConstrainedAStar
    {
        public const int ModeVh = 0; 
        public const int ModeN = 1; 

        public static int mode = ModeVh;

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

        private Node GetMinCostNode(List<Node> nodes)
        {
            Node node = nodes[0];

            for (int i = 1; i < nodes.Count; i++)
            {
                float pCostA = nodes[i].GCost + nodes[i].HCost;
                float pCostB = node.GCost + node.HCost;

                if (pCostA < pCostB)
                {
                    node = nodes[i];
                }

                if (mode == ModeVh)
                {
                    if (Math.Abs(pCostA - pCostB) < Mathf.Epsilon)
                    {
                        if (nodes[i].VCost < node.VCost)
                        {
                            node = nodes[i];
                        }
                    }
                }
                else if (mode == ModeN)
                {
                    if (Math.Abs(pCostA - pCostB) < Mathf.Epsilon)
                    {
                        if (nodes[i].HCost < node.HCost)
                        {
                            node = nodes[i];
                        }
                    }
                }
            }

            return node;
        }

        private int GetVCost(Node n, List<List<Node>> solution, int curAgent)
        {
            if (solution == null || curAgent < 0)
            {
                return 0;
            }

            int vcost = 0;
            for (int i = 0; i < solution.Count; i++)
            {
                if (i != curAgent)
                {
                    // コンフリクトのコストを加算
                    List<Node> path = solution[i];
                    if (n.Time < path.Count && path[n.Time] == n)
                    {
                        vcost++;
                    }
                }
            }

            return vcost;
        }


        public List<Node> FindPath(
            int start,
            int end,
            List<Constraint> constraints,
            List<List<Node>> prevSolution,
            int agent
        )
        {
            Reset();

            Node startNode = nodes[start];
            Node targetNode = nodes[end];
    
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = GetMinCostNode(openSet);

                openSet.Remove(node);
                closedSet.Add(node);

                //ゴールに到達したら
                if (node == targetNode)
                {
                    //親まで辿ってパスを返す
                    return RetracePath(startNode, targetNode);
                }

                foreach (int nodeIndex in graph.GetNextNodes(node.Index))
                {
                    Node neighbour = nodes[nodeIndex];
                    
                    //制約に引っかかったらスキップ
                    if (constraints.Exists(state => state.Node.Position == neighbour.Position && state.Time == node.Time + 1))
                    {
                        continue;
                    }

                    //探索済みだったらスキップ
                    if (!neighbour.Available || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //g(x) + h(x)
                    float newCostToNeighbour = node.GCost + GetDistance(node, neighbour);

                    //ゴール方向に近づくノードだったら
                    if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);

                        neighbour.Parent = node;
                        neighbour.Time = neighbour.Parent.Time + 1;

                        neighbour.VCost = neighbour.Parent.VCost + GetVCost(neighbour, prevSolution, agent);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            List<Node> path = new List<Node>();
            path.Add(startNode);
            return path;
        }

        private float GetDistance(Node nodeA, Node nodeB)
        {
            float magnitude = Vector2.SqrMagnitude(nodeA.Position - nodeB.Position);
            return magnitude;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Add(startNode);
            path.Reverse();

            return path;
        }

        private void Reset()
        {
            foreach (Node node in nodes)
            {
                node.Reset();
            }
        }
    }
}