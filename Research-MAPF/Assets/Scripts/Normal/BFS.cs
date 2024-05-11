using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class BFS
    {
        private readonly GridGraphMediator mediator;
        private readonly Graph graph;

        public BFS(Graph graph, GridGraphMediator mediator)
        {
            this.mediator = mediator;
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
            Vector2Int goalPos = mediator.GetPos(goal);

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

                    Vector2Int pos = mediator.GetPos(next);

                    float dx = goalPos.x - pos.x;
                    float dy = goalPos.y - pos.y;
                    float dist = dx * dx + dy * dy;

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