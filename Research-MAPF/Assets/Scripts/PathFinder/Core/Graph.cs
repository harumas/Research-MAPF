﻿using System.Collections.Generic;

namespace PathFinder.Core
{
    public class Graph
    {
        private readonly List<int>[] graph;

        public int NodeCount => graph.Length;

        public Graph(int capacity)
        {
            graph = new List<int>[capacity];

            for (var i = 0; i < graph.Length; i++)
            {
                graph[i] = new List<int>(64);
            }
        }

        public void AddEdgeUnique(int from, int to)
        {
            if (!graph[from].Contains(to))
            {
                graph[from].Add(to);
            }
        }

        public void AddEdge(int from, int to)
        {
            graph[from].Add(to);
        }

        public IReadOnlyCollection<int> GetNextNodes(int node)
        {
            return graph[node];
        }
    }
}