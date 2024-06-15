using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Solvers.NormalBFS
{
    public class NormalBFS : ISolver
    {
        private readonly BFS pathFinder;

        public NormalBFS(Graph graph, List<Node> nodes)
        {
            pathFinder = new BFS(graph, nodes);
        }

        public List<(int agentIndex, List<int> path)> Solve(List<SolveContext> contexts)
        {
            var pathList = new List<(int agentIndex, List<int> path)>();

            foreach (SolveContext context in contexts)
            {
                List<int> shortestPath = pathFinder.FindPath(context.Start, context.Goal);
                pathList.Add((context.AgentIndex, shortestPath));
            }

            //距離が近い順にソート
            var orderedList = pathList.OrderBy(item => item.path.Count).ToList();
            
            SolveConflict(orderedList);

            return orderedList;
        }

        private void SolveConflict(List<(int agentIndex, List<int> path)> pathList)
        {
            //最大距離を取得
            int maxMove = pathList.Select(item => item.path.Count).Max();

            for (int t = 0; t < maxMove; t++)
            {
                for (var i = 1; i < pathList.Count; i++)
                {
                    List<int> current = pathList[i].path;

                    if (t >= current.Count)
                    {
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        List<int> target = pathList[j].path;
                        if (t >= target.Count)
                        {
                            continue;
                        }

                        //エージェントが衝突したらタイミングをずらす
                        if (current[t] == target[t])
                        {
                            current.Insert(t, current[t - 1]);
                        }
                    }
                }
            }
        }
    }
}