using System.Collections.Generic;
using UnityEngine;

namespace PathFinding.CBS
{
    public class ConflictFinder
    {
        public List<Conflict> GetConflicts(List<List<Node>> paths)
        {
            List<Conflict> conflicts = new List<Conflict>();

            bool conflictFound = false;
            bool agentLeft = false;

            //時間tにおいて
            for (int t = 0; !conflictFound; t++, agentLeft = false)
            {
                //エージェントiと
                for (int i = 0; i < paths.Count && !conflictFound; i++)
                {
                    //エージェントjの比較
                    for (int j = i + 1; j < paths.Count && !conflictFound; j++)
                    {
                        //同じエージェントは比較しない
                        if (i == j)
                        {
                            continue;
                        }

                        //両方ともゴールに到達していない場合
                        if (t < paths[i].Count && t < paths[j].Count)
                        {
                            agentLeft = true;

                            //同じ時間に同じマスに存在する場合(vertex conflict)
                            if (TryConflictSameTime(paths, t, i, j, out Conflict tConflict))
                            {
                                conflicts.Add(tConflict);
                                conflictFound = true;
                                continue;
                            }

                            //すれ違ったら
                            if (TryConflictCrossing(paths, t, i, j, out Conflict cConflictI, out Conflict cConflictJ))
                            {
                                conflicts.Add(cConflictI);
                                conflicts.Add(cConflictJ);
                                conflictFound = true;
                                continue;
                            }

                            //同じマスに入れ違いになったら(following conflict)
                            // if (TryConflictFollow(paths, t, i, j, out Conflict fConflict))
                            // {
                            //     conflicts.Add(fConflict);
                            //     conflictFound = true;
                            //     continue;
                            // }
                        }

                        if (TryStaticConflict(paths, t, i, j, out Conflict sConflict))
                        {
                            conflicts.Add(sConflict);
                            conflictFound = true;
                            continue;
                        }
                    }
                }

                if (!agentLeft) break;
            }

            return conflicts;
        }

        private bool TryConflictSameTime(List<List<Node>> paths, int t, int i, int j, out Conflict conflict)
        {
            /* CASE 1
                0: A B C F
                1: D E C G
                同じ時間に同じマスに存在する場合(vertex conflict)
            */
            if (paths[i][t] == paths[j][t])
            {
                List<int> agents = new List<int>();
                agents.Add(i);
                agents.Add(j);
                conflict = new Conflict(agents, paths[i][t], t);
                return true;
            }

            conflict = null;
            return false;
        }


        private bool TryConflictCrossing(List<List<Node>> paths, int t, int i, int j, out Conflict conflictI, out Conflict conflictJ)
        {
            if (t + 1 < paths[i].Count && t + 1 < paths[j].Count)
            {
                if (paths[i][t] == paths[j][t + 1] && paths[i][t + 1] == paths[j][t])
                {
                    List<int> curAgents = new List<int>();
                    curAgents.Add(i);
                    conflictI = new Conflict(curAgents, paths[i][t + 1], t + 1);

                    curAgents = new List<int>();
                    curAgents.Add(j);
                    conflictJ = new Conflict(curAgents, paths[j][t + 1], t + 1);

                    return true;
                }
            }

            conflictI = null;
            conflictJ = null;
            return false;
        }

        private bool TryConflictFollow(List<List<Node>> paths, int t, int i, int j, out Conflict conflict)
        {
            /* CASE 2
                0: A B C E
                1: D C B F
                同じマスに入れ違いになったら(following conflict)
            */
            //次も動くことができる場合
            if (t + 1 < paths[i].Count)
            {
                //次に移動するマスに前のエージェントがいる場合
                if (paths[i][t + 1] == paths[j][t])
                {
                    List<int> agents = new List<int>();
                    agents.Add(i);
                    conflict = new Conflict(agents, paths[i][t + 1], t + 1);
                    return true;
                }
            }

            conflict = null;
            return false;
        }

        private bool TryStaticConflict(List<List<Node>> paths, int t, int i, int j, out Conflict conflict)
        {
            if (t > 0 && t < paths[j].Count && t >= paths[i].Count && paths[j][t] == paths[i][paths[i].Count - 1])
            {
                List<int> curAgents = new List<int>();
                curAgents.Add(j);
                conflict = new Conflict(curAgents, paths[i][paths[i].Count - 1], t);
                return true;
            }

            if (t > 0 && t < paths[i].Count && t >= paths[j].Count && paths[i][t] == paths[j][paths[j].Count - 1])
            {
                List<int> curAgents = new List<int>();
                curAgents.Add(i);
                conflict = new Conflict(curAgents, paths[j][paths[j].Count - 1], t);
                return true;
            }

            conflict = null;
            return false;
        }
    }
}