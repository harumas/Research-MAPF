using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{
    public class NormalBFS : IFindStrategy
    {
        private readonly BFS pathFinder;

        public NormalBFS(Graph graph, GridGraphMediator mediator)
        {
            pathFinder = new BFS(graph, mediator);
        }

        public List<(Agent agent, List<int> path)> FindSolution(List<SearchContext> contexts)
        {
            var pathList = new List<(Agent agent, List<int> path)>();

            foreach (SearchContext context in contexts)
            {
                List<int> shortestPath = pathFinder.FindPath(context.Start, context.Goal);
                pathList.Add((context.Agent, shortestPath));
            }

            //距離が近い順にソート
            List<(Agent agent, List<int> path)> orderedList = pathList.OrderBy(item => item.path.Count).ToList();

            //最大距離を取得
            int maxMove = orderedList.Select(item => item.path.Count).Max();

            for (int t = 0; t < maxMove; t++)
            {
                for (var i = 1; i < orderedList.Count; i++)
                {
                    List<int> current = orderedList[i].path;

                    if (t >= current.Count)
                    {
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        List<int> target = orderedList[j].path;
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

            return orderedList;
        }
    }
}