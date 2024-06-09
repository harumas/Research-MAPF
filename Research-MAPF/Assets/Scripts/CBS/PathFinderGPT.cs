//
// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// public class Agent
// {
//     public int Id { get; set; }
//     public (int, int) Start { get; set; }
//     public (int, int) Goal { get; set; }
//
//     public Agent(int id, (int, int) start, (int, int) goal)
//     {
//         Id = id;
//         Start = start;
//         Goal = goal;
//     }
// }
//
// public class Node
// {
//     public Dictionary<int, List<(int, int)>> Paths { get; set; }
//     public List<(int, (int, int), (int, int))> Conflicts { get; set; }
//     public int Cost { get; set; }
//
//     public Node()
//     {
//         Paths = new Dictionary<int, List<(int, int)>>();
//         Conflicts = new List<(int, (int, int), (int, int))>();
//         Cost = 0;
//     }
// }
//
// public class CBS
// {
//     private List<Agent> agents;
//     private (int, int) gridSize;
//
//     public CBS(List<Agent> agents, (int, int) gridSize)
//     {
//         this.agents = agents;
//         this.gridSize = gridSize;
//     }
//
//     public List<Node> Solve()
//     {
//         var openList = new List<Node>();
//         var root = new Node();
//
//         // 初期パスを生成 (A*で)
//         foreach (var agent in agents)
//         {
//             root.Paths[agent.Id] = FindPath(agent.Start, agent.Goal);
//         }
//
//         root.Conflicts = DetectConflicts(root.Paths);
//         root.Cost = CalculateCost(root.Paths);
//         openList.Add(root);
//
//         while (openList.Count > 0)
//         {
//             var node = openList.OrderBy(n => n.Cost).First();
//             openList.Remove(node);
//
//             if (node.Conflicts.Count == 0)
//             {
//                 return node.Paths.Values.ToList();
//             }
//
//             var conflict = node.Conflicts.First();
//             var (time, pos1, pos2) = conflict;
//
//             foreach (var agentId in new[] { pos1.Item1, pos2.Item1 })
//             {
//                 var newNode = new Node();
//                 newNode.Paths = new Dictionary<int, List<(int, int)>>(node.Paths);
//                 newNode.Paths[agentId] = FindPathAvoidingConflict(newNode.Paths[agentId], conflict, agentId);
//                 newNode.Conflicts = DetectConflicts(newNode.Paths);
//                 newNode.Cost = CalculateCost(newNode.Paths);
//                 openList.Add(newNode);
//             }
//         }
//
//         return null;
//     }
//
//     private List<(int, int)> FindPath((int, int) start, (int, int) goal)
//     {
//         // A*アルゴリズムを使ってパスを見つける
//         var openSet = new SortedSet<(int f, (int x, int y) pos)> { (0, start) };
//         var cameFrom = new Dictionary<(int, int), (int, int)>();
//         var gScore = new Dictionary<(int, int), int> { [start] = 0 };
//         var fScore = new Dictionary<(int, int), int> { [start] = Heuristic(start, goal) };
//
//         while (openSet.Count > 0)
//         {
//             var current = openSet.First().pos;
//             if (current == goal)
//                 return ReconstructPath(cameFrom, current);
//
//             openSet.Remove(openSet.First());
//
//             foreach (var neighbor in GetNeighbors(current))
//             {
//                 int tentativeGScore = gScore[current] + 1;
//                 if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
//                 {
//                     cameFrom[neighbor] = current;
//                     gScore[neighbor] = tentativeGScore;
//                     fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
//                     if (!openSet.Any(n => n.pos == neighbor))
//                     {
//                         openSet.Add((fScore[neighbor], neighbor));
//                     }
//                 }
//             }
//
//             // 待機の選択肢を追加
//             int tentativeGScoreWait = gScore[current] + 1;
//             if (!gScore.ContainsKey(current) || tentativeGScoreWait < gScore[current])
//             {
//                 cameFrom[current] = current;
//                 gScore[current] = tentativeGScoreWait;
//                 fScore[current] = gScore[current] + Heuristic(current, goal);
//                 if (!openSet.Any(n => n.pos == current))
//                 {
//                     openSet.Add((fScore[current], current));
//                 }
//             }
//         }
//
//         return new List<(int, int)> { start, goal }; // パスが見つからなかった場合、デフォルトのダミーパスを返す
//     }
//
//     private List<(int, int)> FindPathAvoidingConflict(List<(int, int)> path, (int, (int, int), (int, int)) conflict, int agentId)
//     {
//         var (time, pos1, pos2) = conflict;
//         var start = path.First();
//         var goal = path.Last();
//
//         var openSet = new SortedSet<(int f, (int x, int y) pos)> { (0, start) };
//         var cameFrom = new Dictionary<(int, int), (int, int)>();
//         var gScore = new Dictionary<(int, int), int> { [start] = 0 };
//         var fScore = new Dictionary<(int, int), int> { [start] = Heuristic(start, goal) };
//
//         while (openSet.Count > 0)
//         {
//             var current = openSet.First().pos;
//             if (current == goal)
//                 return ReconstructPath(cameFrom, current);
//
//             openSet.Remove(openSet.First());
//
//             foreach (var neighbor in GetNeighbors(current))
//             {
//                 int tentativeGScore = gScore[current] + 1;
//                 if (tentativeGScore == time && (neighbor == pos1.Item2 || neighbor == pos2.Item2))
//                     continue;
//
//                 if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
//                 {
//                     cameFrom[neighbor] = current;
//                     gScore[neighbor] = tentativeGScore;
//                     fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
//                     if (!openSet.Any(n => n.pos == neighbor))
//                     {
//                         openSet.Add((fScore[neighbor], neighbor));
//                     }
//                 }
//             }
//
//             // 待機の選択肢を追加
//             int tentativeGScoreWait = gScore[current] + 1;
//             if (tentativeGScoreWait != time || (current != pos1.Item2 && current != pos2.Item2))
//             {
//                 if (!gScore.ContainsKey(current) || tentativeGScoreWait < gScore[current])
//                 {
//                     cameFrom[current] = current;
//                     gScore[current] = tentativeGScoreWait;
//                     fScore[current] = gScore[current] + Heuristic(current, goal);
//                     if (!openSet.Any(n => n.pos == current))
//                     {
//                         openSet.Add((fScore[current], current));
//                     }
//                 }
//             }
//         }
//
//         return new List<(int, int)> { start, goal }; // パスが見つからなかった場合、デフォルトのダミーパスを返す
//     }
//
//     private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
//     {
//         var totalPath = new List<(int, int)> { current };
//         while (cameFrom.ContainsKey(current) && cameFrom[current] != current)
//         {
//             current = cameFrom[current];
//             totalPath.Insert(0, current);
//         }
//         return totalPath;
//     }
//
//     private List<(int, int)> GetNeighbors((int, int) node)
//     {
//         var neighbors = new List<(int, int)>
//         {
//             (node.Item1 - 1, node.Item2),
//             (node.Item1 + 1, node.Item2),
//             (node.Item1, node.Item2 - 1),
//             (node.Item1, node.Item2 + 1)
//         };
//
//         return neighbors.Where(n => n.Item1 >= 0 && n.Item1 < gridSize.Item1 && n.Item2 >= 0 && n.Item2 < gridSize.Item2).ToList();
//     }
//
//     private int Heuristic((int, int) a, (int, int) b)
//     {
//         return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
//     }
//
//     private List<(int, (int, int), (int, int))> DetectConflicts(Dictionary<int, List<(int, int)>> paths)
//     {
//         var conflicts = new List<(int, (int, int), (int, int))>();
//
//         foreach (var path1 in paths)
//         {
//             foreach (var path2 in paths)
//             {
//                 if (path1.Key >= path2.Key) continue;
//
//                 for (int t = 0; t < Math.Min(path1.Value.Count, path2.Value.Count); t++)
//                 {
//                     if (path1.Value[t] == path2.Value[t])
//                     {
//                         conflicts.Add((t, (path1.Key, path1.Value[t]), (path2.Key, path2.Value[t])));
//                     }
//
//                     // エージェントの衝突を避けるための追加チェック（エージェントが入れ替わる場合）
//                     if (t + 1 < path1.Value.Count && t + 1 < path2.Value.Count &&
//                         path1.Value[t] == path2.Value[t + 1] && path1.Value[t + 1] == path2.Value[t])
//                     {
//                         conflicts.Add((t + 1, (path1.Key, path1.Value[t + 1]), (path2.Key, path2.Value[t + 1])));
//                     }
//                 }
//             }
//         }
//
//         return conflicts;
//     }
//
//     private int CalculateCost(Dictionary<int, List<(int, int)>> paths)
//     {
//         // 例えばパスの長さの総和をコストとする
//         return paths.Values.Sum(p => p.Count);
//     }
// }
//
// public class Program
// {
//     public static void Main()
//     {
//         var agents = new List<Agent>
//         {
//             new Agent(1, (0, 0), (2, 2)),
//             new Agent(2, (2, 0), (0, 2))
//         };
//
//         var cbs = new CBS(agents, (3, 3));
//         var solution = cbs.Solve();
//
//         if (solution != null)
//         {
//             foreach (var path in solution)
//             {
//                 Console.WriteLine(string.Join(" -> ", path.Select(p => $"({p.Item1}, {p.Item2})")));
//             }
//         }
//         else
//         {
//             Console.WriteLine("解が見つかりませんでした。");
//         }
//     }
// }
