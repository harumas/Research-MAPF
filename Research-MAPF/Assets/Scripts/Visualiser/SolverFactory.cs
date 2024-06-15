﻿using System.Collections.Generic;
using PathFinder.Core;
using PathFinder.Solvers.CBS;
using PathFinder.Solvers.NormalBFS;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Visualiser
{
    public class SolverFactory
    {
        private readonly Graph graph;
        private readonly GridGraphMediator mediator;

        public SolverFactory(Graph graph, GridGraphMediator mediator)
        {
            this.graph = graph;
            this.mediator = mediator;
        }

        public Dictionary<FindStrategy, ISolver> CreateSolvers()
        {
            List<Node> nodes = new List<Node>(graph.NodeCount);

            for (int i = 0; i < graph.NodeCount; i++)
            {
                Vector2Int pos = mediator.GetPos(i);
                nodes.Add(new Node(i, new Vector2(pos.x, pos.y)));
            }

            var solvers = new Dictionary<FindStrategy, ISolver>()
            {
                { FindStrategy.NormalBFS, new NormalBFS(graph, nodes) },
                { FindStrategy.CBS, new CBS(graph, nodes) }
            };

            return solvers;
        }
    }
}