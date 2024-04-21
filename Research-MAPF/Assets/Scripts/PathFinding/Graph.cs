using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Graph
    {
        private readonly HashSet<int>[] graph;

        public int NodeCount => graph.Length;

        public Graph(int capacity)
        {
            graph = new HashSet<int>[capacity];

            for (var i = 0; i < graph.Length; i++)
            {
                graph[i] = new HashSet<int>(64);
            }
        }

        public void AddEdge(int from, int to)
        {
            if (!graph[from].Add(to))
            {
                Debug.LogError($"重複している要素は追加できません！element: {to}");      
            }
        }

        public IReadOnlyCollection<int> GetNextNodes(int node)
        {
            return graph[node];
        }
    }
}