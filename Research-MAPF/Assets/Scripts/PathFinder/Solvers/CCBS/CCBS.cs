using System.Collections;
using System.Collections.Generic;
using PathFinder.Core;
using PathFinder.Solvers.CCBS;
using UnityEngine;

public class CCBS : ISolver
{
    private readonly ConstraindSIPP constraindSipp;

    private float agentRadius = 0.5f;
    private float agentSpeed = 1f;

    public CCBS(Graph graph, List<Node> nodes)
    {
        constraindSipp = new ConstraindSIPP(graph);
    }

    public List<(int agentIndex, List<int> path)> Solve(List<SolveContext> contexts)
    {
        List<List<int>> testPath = new List<List<int>>();
        testPath.Add(new List<int>() { 0, 1, 2, 3, 4 });
        testPath.Add(new List<int>() { 5, 6, 7, 2, 4, 7 });

        constraindSipp.ConstructInterval(testPath);
        return null;
    }
}