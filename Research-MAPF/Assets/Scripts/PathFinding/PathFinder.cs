using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private GridGraphMediator mediator;
        [SerializeField] private Vector2Int start;
        [SerializeField] private Vector2Int end;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private Color pathColor;

        private Graph graph;

        private void Start()
        {
            mediator.Initialize();
            graph = mediator.ConstructGraph();

            int startNode = mediator.GetNode(start);
            int endNode = mediator.GetNode(end);

            //スタートとゴールに色を付ける
            mediator.GetCell(startNode).SetColor(startColor);
            mediator.GetCell(endNode).SetColor(endColor);
        }

        public void FindPath()
        {
            int startNode = mediator.GetNode(start);
            int endNode = mediator.GetNode(end);

            //最短経路を探索
            List<int> shortestPath = SearchShortestPath(startNode, endNode);

            //startとgoalを除いた部分を塗る
            for (var i = 1; i < shortestPath.Count - 1; i++)
            {
                var node = shortestPath[i];
                mediator.GetCell(node).SetColor(pathColor);
            }
        }

        private List<int> SearchShortestPath(int start, int goal)
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

                //接続しているノードを取得
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

            //ゴールからスタートまでのパスを辿る
            while (current != start)
            {
                foreach (int next in graph.GetNextNodes(current))
                {
                    if (distanceList[next] == distance)
                    {
                        shortestPath.Add(next);
                        distance = distanceList[next] - 1;
                        current = next;
                        break;
                    }
                }
            }

            //逆転してスタートからゴールのパスに変換
            shortestPath.Reverse();

            return shortestPath;
        }
    }
}