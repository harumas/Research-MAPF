using System;
using System.Collections.Generic;
using System.Numerics;
using PathFinder.Core;

namespace PathFinder.Solvers.NormalBFS
{
    public class BFS
    {
        private readonly List<Node> nodes;
        private readonly Graph graph;

        public BFS(Graph graph, List<Node> nodes)
        {
            this.nodes = nodes;
            this.graph = graph;
        }

        public List<int> FindPath(int start, int goal)
        {
            //距離を保持する配列(探索していないノードは-1)
            int[] distanceList = new int[graph.NodeCount];
            Array.Fill(distanceList, -1);

            Queue<int> searchNodes = new Queue<int>();

            //初めはスタートを登録
            distanceList[start] = 0;
            searchNodes.Enqueue(start);

            while (searchNodes.Count > 0)
            {
                //探索するノードを取得
                int node = searchNodes.Dequeue();

                //ゴールに到達したらパスを構築する
                if (node == goal)
                {
                    return ConstructShortestPath(distanceList, start, goal);
                }

                //隣接しているノードを取得
                foreach (int next in graph.GetNextNodes(node))
                {
                    //探索していたらスキップ
                    if (distanceList[next] != -1)
                    {
                        continue;
                    }

                    //次のノードに距離を加算して探索ノードに加える
                    distanceList[next] = distanceList[node] + 1;
                    searchNodes.Enqueue(next);
                }
            }

            return new List<int>();
        }

        private List<int> ConstructShortestPath(int[] distanceList, int start, int goal)
        {
            List<int> shortestPath = new List<int>();
            shortestPath.Add(goal);

            int distance = distanceList[goal] - 1;
            int current = goal;
            Vector2 goalPos = nodes[goal].Position;

            //ゴールからスタートまでのパスを辿る
            while (current != start)
            {
                int minNode = -1;
                float minDistance = float.MaxValue;

                foreach (int next in graph.GetNextNodes(current))
                {
                    if (distanceList[next] != distance)
                    {
                        continue;
                    }

                    Vector2 pos = nodes[next].Position;

                    float dist = (goalPos - pos).LengthSquared();

                    //できるだけゴールに近いノードを選択する
                    if (dist < minDistance)
                    {
                        minNode = next;
                        minDistance = dist;
                    }
                }

                shortestPath.Add(minNode);
                distance = distanceList[minNode] - 1;
                current = minNode;
            }

            //逆転してスタートからゴールのパスに変換
            shortestPath.Reverse();

            return shortestPath;
        }
    }
}